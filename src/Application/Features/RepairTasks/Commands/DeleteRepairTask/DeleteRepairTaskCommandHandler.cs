using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Common.Results;
using Domain.Features.RepairTasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Application.Features.RepairTasks.Commands.DeleteRepairTask
{
    public class DeleteRepairTaskCommandHandler : IRequestHandler<DeleteRepairTaskCommand, Result<Deleted>>
    {
        private readonly IAppDbContext context;
        private readonly HybridCache cache;
        private readonly ILogger<DeleteRepairTaskCommandHandler> logger;

        public DeleteRepairTaskCommandHandler(IAppDbContext context, HybridCache cache, ILogger<DeleteRepairTaskCommandHandler> logger)
        {
            this.cache = cache;
            this.logger = logger;
            this.context = context;
        }
        public async Task<Result<Deleted>> Handle(DeleteRepairTaskCommand request, CancellationToken cancellationToken)
        {
            RepairTask? repairTaskToBeDeleted = await context.RepairTasks.Include(rt=>rt.Parts).FirstOrDefaultAsync(rt=>rt.Id ==request.RepairTaskToBeDeletedId, cancellationToken);

            if(repairTaskToBeDeleted is null)
            {
                logger.LogWarning("repair task which intended to be deleted was not found, its id: {repairtaskId}", request.RepairTaskToBeDeletedId);

                return ApplicationErrors.RepairTaskNotFound;
            }
            bool IsRepairTaskUsedInWorkOrders = await context.WorkOrders.SelectMany(wo=>wo.RepairTasks).AnyAsync(wort=>wort.Id ==request.RepairTaskToBeDeletedId, cancellationToken);

            if(IsRepairTaskUsedInWorkOrders)
            {
                logger.LogWarning("the repair task with id: {repairtaskId} is used in work orders, cannot be deleted", request.RepairTaskToBeDeletedId);

                return RepairTaskErrors.InUse;
            }

            context.RepairTasks.Remove(repairTaskToBeDeleted);
            await context.SaveChangesAsync(cancellationToken);

            await cache.RemoveByTagAsync("repair-tasks", cancellationToken);
            logger.LogInformation("RepairTask {repairtaskId} deleted successfully.",request.RepairTaskToBeDeletedId);

            return Result.Deleted;
        }
    }
}