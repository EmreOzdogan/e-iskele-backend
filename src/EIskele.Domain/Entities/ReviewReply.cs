using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class ReviewReply : BaseEntity, IAuditableEntity
{
    public Guid ReviewId { get; set; }
    public Review Review { get; set; } = null!;

    public string ReplyText { get; set; } = string.Empty;

}
