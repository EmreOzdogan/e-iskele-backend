using EIskele.Domain.Common;
using EIskele.Domain.Enums;

namespace EIskele.Domain.Entities;

public class SupportTicket : BaseEntity, IAuditableEntity, ISoftDeletableEntity
{
    public string TicketNo { get; set; } = string.Empty;
    
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    
    public string Subject { get; set; } = string.Empty;
    public SupportTicketCategory Category { get; set; }
    public SupportTicketPriority Priority { get; set; }
    public SupportTicketStatus Status { get; set; } = SupportTicketStatus.Open;
    
    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    // ISoftDeletableEntity
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
