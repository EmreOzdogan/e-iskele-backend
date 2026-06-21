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

}
