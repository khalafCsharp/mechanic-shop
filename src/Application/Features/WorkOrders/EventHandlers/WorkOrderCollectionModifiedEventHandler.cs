using Application.Common.Interfaces;
using Domain.Features.WorkOrders.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.WorkOrders.EventHandlers
{
    public class WorkOrderCollectionModifiedEventHandler : INotificationHandler<WorkOrderCollectionModified>
    {
        private readonly IWorkOrderNotifier notifier;
        private readonly ILogger<WorkOrderCollectionModifiedEventHandler> logger;

        public WorkOrderCollectionModifiedEventHandler(IWorkOrderNotifier notifier, ILogger<WorkOrderCollectionModifiedEventHandler> logger)
        {
            this.notifier = notifier;
            this.logger = logger;
        }
        public Task Handle(WorkOrderCollectionModified notification, CancellationToken cancellationToken)
        {
            logger.LogError("we are about to call the signalr hub method");
           return notifier.NotifyWorkOrdersChangedAsync(cancellationToken);
        }
    }
}