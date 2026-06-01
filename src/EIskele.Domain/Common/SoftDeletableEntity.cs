namespace EIskele.Domain.Common;

public abstract class SoftDeletableEntity : BaseEntity, ISoftDeletableEntity
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
