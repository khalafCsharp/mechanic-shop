using Application.Common.Interfaces;
using Domain.Features.WorkOrders.Events;
using MediatR;

namespace Application.Features.WorkOrders.EventHandlers
{
    public class WorkOrderCollectionModifiedEventHandler : INotificationHandler<WorkOrderCollectionModified>
    {
        private readonly IWorkOrderNotifier notifier;
        public WorkOrderCollectionModifiedEventHandler(IWorkOrderNotifier notifier)
        {
            this.notifier = notifier;

        }
        public Task Handle(WorkOrderCollectionModified notification, CancellationToken cancellationToken)
        {
           return notifier.NotifyWorkOrdersChangedAsync(cancellationToken);
        }
    }
}