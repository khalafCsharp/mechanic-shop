using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Common.Results;
using Domain.Common.Results.Errors;
using Domain.Features.RepairTasks;
using Domain.Features.WorkOrders;
using Domain.Features.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Application.Features.WorkOrders.Commands.UpdateWorkOrderRepairTasks
{
    public class UpdateWorkOrderRepairTasksCommandHandler : IRequestHandler<UpdateWorkOrderRepairTasksCommand, Result<Updated>>
    {
        private readonly IAppDbContext context;
        private readonly ILogger<UpdateWorkOrderRepairTasksCommandHandler> logger;
        private readonly HybridCache cache;
        private readonly IWorkOrderPolicy workOrderPolicy;

        public UpdateWorkOrderRepairTasksCommandHandler(IAppDbContext context, ILogger<UpdateWorkOrderRepairTasksCommandHandler> logger, HybridCache cache, IWorkOrderPolicy workOrderPolicy)
        {
            this.workOrderPolicy = workOrderPolicy;
            this.logger = logger;
            this.cache = cache;
            this.context = context;
        }
        public async Task<Result<Updated>> Handle(UpdateWorkOrderRepairTasksCommand request, CancellationToken cancellationToken)
        {
            WorkOrder? workOrderToBeUpdated = await context.WorkOrders
           .Include(w => w.RepairTasks)
           .FirstOrDefaultAsync(w => w.Id == request.WorkOrderId, cancellationToken);

            if (workOrderToBeUpdated is null)
            {
                logger.LogError("WorkOrder with id: {workOrderId} does not exist.", request.WorkOrderId);

                return ApplicationErrors.WorkOrderNotFound;
            }

            if (request.RepairTaskIds.Length == 0)
            {
                logger.LogError("error: Empty RepairTaskIds list submitted.");

                return RepairTaskErrors.AtLeastOneRepairTaskIsRequired;
            }

            List<RepairTask> requestedTasks = await context.RepairTasks
                       .Where(t => request.RepairTaskIds.Contains(t.Id))
                       .ToListAsync(cancellationToken);

            if (requestedTasks.Count != request.RepairTaskIds.Length)
            {
                var idsDoNotHaveRepairTask = request.RepairTaskIds.Except(requestedTasks.Select(t => t.Id)).ToArray();

                logger.LogError("One or more repair tasks not found. {ids}", string.Join(", ", idsDoNotHaveRepairTask));

                return ApplicationErrors.RepairTaskNotFound;
            }

            var clearExistingResult = workOrderToBeUpdated.ClearRepairTasks();

            if (clearExistingResult.IsError)
            {
                return clearExistingResult.Errors;
            }

            foreach (RepairTask task in requestedTasks)
            {
                var addRepairTaskResult = workOrderToBeUpdated.AddRepairTask(task);

                if (addRepairTaskResult.IsError)
                {
                    return addRepairTaskResult.Errors;
                }
            }

            var totalDuration = TimeSpan.FromMinutes(requestedTasks.Sum(x => (int)x.EstimatedDurationInMins));

            var newEndAt = workOrderToBeUpdated.StartAtUtc + totalDuration;

            if (workOrderPolicy.IsOutsideOperatingHours(workOrderToBeUpdated.StartAtUtc, totalDuration))
            {
                return Error.Conflict("WorkOrder_Outside_OperatingHours", "WorkOrder timing exceeds business hours.");
            }

            var spotCheckResult = await workOrderPolicy.CheckSpotAvailabilityAsync(
                workOrderToBeUpdated.Spot,
                workOrderToBeUpdated.StartAtUtc,
                newEndAt,
                 workOrderToBeUpdated.Id,
                cancellationToken);

            if (spotCheckResult.IsError)
            {
                return spotCheckResult.Errors;
            }

            if (await workOrderPolicy.IsLaborOccupied(workOrderToBeUpdated.LaborId, workOrderToBeUpdated.Id, workOrderToBeUpdated.StartAtUtc, newEndAt))
            {
                return ApplicationErrors.LaborOccupied;
            }
            workOrderToBeUpdated.UpdateTiming(workOrderToBeUpdated.StartAtUtc, newEndAt);


            workOrderToBeUpdated.AddDomainEvent(new WorkOrderCollectionModified());
            
            await context.SaveChangesAsync(cancellationToken);


            await cache.RemoveByTagAsync("work-orders", cancellationToken);

            return Result.Updated;
        }
    }
}