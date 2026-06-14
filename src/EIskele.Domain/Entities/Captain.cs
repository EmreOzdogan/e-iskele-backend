using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class Captain : SoftDeletableEntity
{
    public Guid UserId { get; set; }
    public string ApplicationNo { get; set; } = string.Empty;
    public string ApplicationType { get; set; } = "Individual"; // Individual, Company
    public string IdentityNumber { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft"; // Draft, UnderReview, Approved, Rejected, Suspended
    public string AccountStatus { get; set; } = "Active"; // Active, Passive, Suspended
    public string Bio { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Harbor { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? Iban { get; set; }
    public string? AdminNote { get; set; }
    
    // Navigation
    public ApplicationUser User { get; set; } = null!;
    public ICollection<Boat> Boats { get; set; } = new List<Boat>();
    public Company? Company { get; set; }
}
