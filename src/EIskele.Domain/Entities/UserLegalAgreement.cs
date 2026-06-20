using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class UserLegalAgreement : BaseEntity
{
    public Guid UserId { get; set; }
    public string AgreementName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Status { get; set; } = "accepted"; // accepted, declined

    // Navigation
    public ApplicationUser User { get; set; } = null!;
}
