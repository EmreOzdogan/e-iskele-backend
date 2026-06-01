using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class Captain : SoftDeletableEntity
{
    public Guid UserId { get; set; }
    public string IdentityNumber { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft"; // Draft, UnderReview, Approved, Rejected
    public string Bio { get; set; } = string.Empty;
    public string? AdminNote { get; set; }
    
    // Navigation
    public ApplicationUser User { get; set; } = null!;
    public ICollection<Boat> Boats { get; set; } = new List<Boat>();
}
