using Application.Common.Errors;
using Application.Common.Interfaces;
using Application.Features.RepairTasks.Dtos;
using Domain.Common.Results;
using Domain.Features.RepairTasks;
using Domain.Features.RepairTasks.Parts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Application.Features.RepairTasks.Commands.UpdateRepairTask
{
    public class UpdateRepairTaskCommandHandler : IRequestHandler<UpdateRepairTaskCommand, Result<Updated>>
    {
        private readonly IAppDbContext context;
        private readonly ILogger<UpdateRepairTaskCommandHandler> logger;
        private readonly HybridCache cache;

        public UpdateRepairTaskCommandHandler(IAppDbContext context, ILogger<UpdateRepairTaskCommandHandler> logger, HybridCache cache)
        {
            this.logger = logger;
            this.cache = cache;
            this.context = context;
        }
        public async Task<Result<Updated>> Handle(UpdateRepairTaskCommand request, CancellationToken cancellationToken)
        {

            RepairTask? repairTaskToBeUpdated = await context.RepairTasks
                .Include(rt => rt.Parts)
                .FirstOrDefaultAsync(rt => rt.Id == request.RepairTaskId, cancellationToken);

            if (repairTaskToBeUpdated is null)
            {
                logger.LogWarning("RepairTask {RepairTaskId} not found for update.", request.RepairTaskId);

                return ApplicationErrors.RepairTaskNotFound;
            }

            List<Part> validatedParts = new List<Part>();
            foreach (UpdatePartCommand p in request.UpdatePartCommands)
            {
                var partId = p.PartId ?? Guid.NewGuid();

                var partResult = Part.Create(partId, p.Name, p.Cost, p.Quantity);

                if (partResult.IsError)
                {
                    return partResult.Errors;
                }

                validatedParts.Add(partResult.Value);
            }
            var updateRepairTaskResult = repairTaskToBeUpdated.Update(request.Name, request.LaborCost, request.TaskDuration);

            if (updateRepairTaskResult.IsError)
            {
                return updateRepairTaskResult.Errors;
            }

            var upsertPartsResult = repairTaskToBeUpdated.UpdateRepairTaskParts(validatedParts);

            if (upsertPartsResult.IsError)
            {
                return upsertPartsResult.Errors;
            }

            await context.SaveChangesAsync(cancellationToken);

            await cache.RemoveByTagAsync("repair-tasks", cancellationToken);

            return Result.Updated;
        }
    }
}