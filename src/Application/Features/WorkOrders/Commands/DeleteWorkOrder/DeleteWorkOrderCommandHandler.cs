using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Common.Results;
using Domain.Features.WorkOrders;
using Domain.Features.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Application.Features.WorkOrders.Commands.DeleteWorkOrder
{
    public class DeleteWorkOrderCommandHandler : IRequestHandler<DeleteWorkOrderCommand, Result<Deleted>>
    {
        private readonly IAppDbContext context;
        private readonly HybridCache cache;
        private readonly ILogger<DeleteWorkOrderCommandHandler> logger;

        public DeleteWorkOrderCommandHandler(IAppDbContext context, HybridCache cache, ILogger<DeleteWorkOrderCommandHandler> logger)
        {
            this.context = context;
            this.cache = cache;
            this.logger = logger;
        }
        public async Task<Result<Deleted>> Handle(DeleteWorkOrderCommand request, CancellationToken cancellationToken)
        {
            WorkOrder? workOrderToBeDeleted = await context.WorkOrders.FirstOrDefaultAsync(wo => wo.Id == request.WorkOrderId, cancellationToken);

            if (workOrderToBeDeleted is null)
            {
                logger.LogError("work order to be deleted not found Id: {workOrderId}", request.WorkOrderId);

                return ApplicationErrors.WorkOrderNotFound;
            }
            if (workOrderToBeDeleted.State != WorkOrderState.Scheduled)
            {
                logger.LogError("work order with id: {workOrderId} is not in deletable state", request.WorkOrderId);

                return WorkOrderErrors.Readonly;
            }

            context.WorkOrders.Remove(workOrderToBeDeleted);


            workOrderToBeDeleted.AddDomainEvent(new WorkOrderCollectionModified());
            logger.LogError(
    "WorkOrder domain events before SaveChanges: {Count}",
    workOrderToBeDeleted.DomainEvents.Count);

            await context.SaveChangesAsync(cancellationToken);

            await cache.RemoveByTagAsync("work-orders", cancellationToken);

            return Result.Deleted;
        }
    }
}