using EIskele.Domain.Common;
using EIskele.Domain.Enums;

namespace EIskele.Domain.Entities;

public class UserAdminNote : BaseEntity, IAuditableEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    
    public NoteType NoteType { get; set; }
    public string Note { get; set; } = string.Empty;
    
}
