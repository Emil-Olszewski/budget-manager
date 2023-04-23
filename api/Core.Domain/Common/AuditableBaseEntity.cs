namespace Core.Domain.Common
{
    public abstract class AuditableBaseEntity : BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
    }
}