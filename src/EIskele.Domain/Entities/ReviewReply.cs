using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class ReviewReply : BaseEntity, IAuditableEntity
{
    public Guid ReviewId { get; set; }
    public Review Review { get; set; } = null!;

    public string ReplyText { get; set; } = string.Empty;

    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
}
