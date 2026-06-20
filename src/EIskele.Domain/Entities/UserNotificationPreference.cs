using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class UserNotificationPreference : BaseEntity
{
    public Guid UserId { get; set; }
    public string Category { get; set; } = string.Empty; // e.g. "newReservation", "payments"
    public bool Email { get; set; }
    public bool Sms { get; set; }
    public bool Whatsapp { get; set; }
    public bool InApp { get; set; }

    // Navigation
    public ApplicationUser User { get; set; } = null!;
}
