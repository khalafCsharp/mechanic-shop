using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Common.Results;
using Domain.Features.WorkOrders;
using Domain.Features.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Application.Features.WorkOrders.Commands.UpdateWorkOrderState
{
    public class UpdateWorkOrderStateCommandHandler : IRequestHandler<UpdateWorkOrderStateCommand, Result<Updated>>
    {
        private readonly IAppDbContext context;
        private readonly ILogger<UpdateWorkOrderStateCommandHandler> logger;
        private readonly HybridCache cache;
        private readonly TimeProvider dateTime;

        public UpdateWorkOrderStateCommandHandler(IAppDbContext context, ILogger<UpdateWorkOrderStateCommandHandler> logger, HybridCache cache, TimeProvider dateTime)
        {
            this.dateTime = dateTime;
            this.context = context;
            this.logger = logger;
            this.cache = cache;
        }
        public async Task<Result<Updated>> Handle(UpdateWorkOrderStateCommand request, CancellationToken cancellationToken)
        {
            var workOrderToBeUpdated = await context.WorkOrders
           .FirstOrDefaultAsync(a => a.Id == request.WorkOrderId, cancellationToken);

            if (workOrderToBeUpdated is null)
            {
                logger.LogError("WorkOrder with id: {workorderId} does not exist.", request.WorkOrderId);

                return ApplicationErrors.WorkOrderNotFound;
            }

            if(workOrderToBeUpdated.StartAtUtc > dateTime.GetUtcNow())
            {
                logger.LogError("cannot change work order's state before its commence time for work order id: {workOrderId}", workOrderToBeUpdated.Id);

                return WorkOrderErrors.StateTransitionNotAllowed(workOrderToBeUpdated.StartAtUtc);
            }

            var updateStatusResult = workOrderToBeUpdated.UpdateState(request.NewState);

            if (updateStatusResult.IsError)
            {
                logger.LogError("Failed to update state: {Error}", updateStatusResult.TopError.Description);

                return updateStatusResult.Errors;
            }
           

            if(request.NewState == WorkOrderState.Completed)
            {
                workOrderToBeUpdated.AddDomainEvent(new WorkOrderCompleted(){WorkOrderId = request.WorkOrderId});
            }
            workOrderToBeUpdated.AddDomainEvent(new WorkOrderCollectionModified());

            await context.SaveChangesAsync(cancellationToken);
            
            await cache.RemoveByTagAsync("work-orders", cancellationToken);

            return Result.Updated;
        }
    }
}