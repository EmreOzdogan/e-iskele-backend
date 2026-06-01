namespace EIskele.Domain.Common;

public interface ISoftDeletableEntity : IAuditableEntity
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    Guid? DeletedBy { get; set; }
}
