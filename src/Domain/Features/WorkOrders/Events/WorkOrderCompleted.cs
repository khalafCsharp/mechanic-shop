using Domain.Common.Entities;

namespace Domain.Features.WorkOrders.Events
{
    public sealed class WorkOrderCompleted : DomainEvent
    {
        public Guid WorkOrderId { get; set; }
    }
}