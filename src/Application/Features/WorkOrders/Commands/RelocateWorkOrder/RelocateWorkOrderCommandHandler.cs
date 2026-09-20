using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Common.Results;
using Domain.Features.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Application.Features.WorkOrders.Commands.RelocateWorkOrder
{
    public class RelocateWorkOrderCommandHandler : IRequestHandler<RelocateWorkOrderCommand, Result<Updated>>
    {
        private readonly IAppDbContext context;
        private readonly ILogger<RelocateWorkOrderCommandHandler> logger;
        private readonly HybridCache cache;
        private readonly IWorkOrderPolicy workOrderPolicy;

        public RelocateWorkOrderCommandHandler(IAppDbContext context, ILogger<RelocateWorkOrderCommandHandler> logger, HybridCache cache, IWorkOrderPolicy workOrderPolicy)
        {
            this.workOrderPolicy = workOrderPolicy;
            this.logger = logger;
            this.cache = cache;
            this.context = context;
        }
        public async Task<Result<Updated>> Handle(RelocateWorkOrderCommand request, CancellationToken cancellationToken)
        {
            var workOrderToBeUpdated = await context.WorkOrders
           .Include(a => a.RepairTasks)
           .Include(a => a.Labor)
           .Include(a => a.Vehicle)
           .FirstOrDefaultAsync(a => a.Id == request.WorkOrderId, cancellationToken);

            if (workOrderToBeUpdated is null)
            {
                logger.LogError("WorkOrder with id: {workOrderId} does not exist.", request.WorkOrderId);

                return ApplicationErrors.WorkOrderNotFound;
            }

            var duration = workOrderToBeUpdated.EndAtUtc.Subtract(workOrderToBeUpdated.StartAtUtc).Duration();

            var endAt = request.NewStartAt.Add(duration);

            var checkSpotAvailabilityResult = await workOrderPolicy.CheckSpotAvailabilityAsync(
            request.NewSpot,
            request.NewStartAt,
            endAt,
            cancellationToken);

            if (checkSpotAvailabilityResult.IsError)
            {
                logger.LogError("Spot: {spot} is not available.", workOrderToBeUpdated.Spot.ToString());

                return checkSpotAvailabilityResult.Errors;
            }

            if (await workOrderPolicy.IsLaborOccupied(workOrderToBeUpdated.LaborId, request.WorkOrderId, request.NewStartAt, endAt))
            {
                logger.LogError("Labor with Id '{LaborId}' is already occupied during the requested time.", workOrderToBeUpdated.LaborId);

                return ApplicationErrors.LaborOccupied;
            }

            if (await workOrderPolicy.IsVehicleAlreadyScheduled(workOrderToBeUpdated.VehicleId, request.NewStartAt, endAt, request.WorkOrderId))
            {
                logger.LogError("Vehicle with Id '{VehicleId}' already has an overlapping WorkOrder.", workOrderToBeUpdated.VehicleId);

                return ApplicationErrors.VehicleSchedulingConflict;
            }

            var updateTimingResult = workOrderToBeUpdated.UpdateTiming(request.NewStartAt, endAt);

            if (updateTimingResult.IsError)
            {
                logger.LogError("Failed to update timing: {Error}", updateTimingResult.TopError.Description);

                return updateTimingResult.Errors;
            }

            var updateSpotResult = workOrderToBeUpdated.UpdateSpot(request.NewSpot);

            if (updateSpotResult.IsError)
            {
                logger.LogError("Failed to update Spot: {Error}", updateSpotResult.TopError.Description);

                return updateSpotResult.Errors;
            }

            await context.SaveChangesAsync(cancellationToken);

            workOrderToBeUpdated.AddDomainEvent(new WorkOrderCollectionModified());

            await cache.RemoveByTagAsync("work-order", cancellationToken);

            return Result.Updated;
        }
    }
}