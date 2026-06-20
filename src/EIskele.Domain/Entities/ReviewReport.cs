using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class ReviewReport : BaseEntity, IAuditableEntity
{
    public Guid ReviewId { get; set; }
    public Review Review { get; set; } = null!;

    public string Reason { get; set; } = string.Empty;
    public string? Message { get; set; }

    public bool IsResolved { get; set; }
    public string? AdminNote { get; set; }

    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
}
