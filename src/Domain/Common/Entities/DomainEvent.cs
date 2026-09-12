using MediatR;

namespace Domain.Common.Entities
{
    public abstract class DomainEvent : INotification;
}