using Microsoft.AspNetCore.Identity;
using EIskele.Domain.Common;
using EIskele.Domain.Enums;

namespace EIskele.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>, ISoftDeletableEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }

    
    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    // ISoftDeletableEntity
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    // User Module Properties
    public string UserNo { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public UserStatus Status { get; set; } = UserStatus.PendingVerification;
    public UserVerificationStatus VerificationStatus { get; set; } = UserVerificationStatus.Unverified;
    public UserRiskStatus RiskStatus { get; set; } = UserRiskStatus.Normal;
    public RegistrationSource RegistrationSource { get; set; } = RegistrationSource.Web;
    
    // Security & Session
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
    public DateTime? LastPasswordChangedAt { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    public DateTime? PhoneVerifiedAt { get; set; }
    public int ActiveSessionCount { get; set; }

    // Navigation
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public Captain? Captain { get; set; }
    public ICollection<UserAdminNote> AdminNotes { get; set; } = new List<UserAdminNote>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();
    public ICollection<UserNotificationPreference> NotificationPreferences { get; set; } = new List<UserNotificationPreference>();
    public ICollection<UserLegalAgreement> LegalAgreements { get; set; } = new List<UserLegalAgreement>();
    public ICollection<UserSecurityEvent> SecurityEvents { get; set; } = new List<UserSecurityEvent>();
    public ICollection<UserActiveSession> ActiveSessions { get; set; } = new List<UserActiveSession>();
}
