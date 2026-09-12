namespace Domain.Common.Entities
{
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTimeOffset CreatedAtUtc { get; set; }
        public DateTimeOffset LastModifiedAtUtc { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }

        protected AuditableEntity(Guid id) : base(id)
        {
            
        }
    }
}