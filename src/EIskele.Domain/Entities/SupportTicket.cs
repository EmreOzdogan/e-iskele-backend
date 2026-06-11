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
    public new DateTime CreatedAt { get; set; }
    public new Guid? CreatedBy { get; set; }
    public new DateTime? UpdatedAt { get; set; }
    public new Guid? UpdatedBy { get; set; }
    
    // ISoftDeletableEntity
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
