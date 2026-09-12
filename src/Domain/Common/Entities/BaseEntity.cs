using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Common.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get;  }
        private readonly List<DomainEvent> _domainEvents = [];
        [NotMapped]
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected BaseEntity(Guid id)
        {
            Id = id ==Guid.Empty ? Guid.NewGuid() : id;
        }

        public void AddDomainEvent(DomainEvent de)
        {
            _domainEvents.Add(de);
        }
        public void RemoveDomainEvent(DomainEvent de)
        {
            _domainEvents.Remove(de);
        }
        public void ClearDomainEvent()
        {
            _domainEvents.Clear();
        }
    }
}