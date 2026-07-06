using EIskele.Domain.Common;
using EIskele.Domain.Enums;

namespace EIskele.Domain.Entities;

public class Captain : SoftDeletableEntity
{
    public Guid UserId { get; set; }
    public string ApplicationNo { get; set; } = string.Empty;
    public CaptainApplicationType ApplicationType { get; set; } = CaptainApplicationType.Individual;
    public string IdentityNumber { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public CaptainStatus Status { get; set; } = CaptainStatus.Draft;
    public CaptainAccountStatus AccountStatus { get; set; } = CaptainAccountStatus.Active;
    public string Bio { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Harbor { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? District { get; set; }

    public string? Iban { get; set; }
    public string? AdminNote { get; set; }
    public int ExperienceYears { get; set; }
    public string? Languages { get; set; }
    
    // Navigation
    public ApplicationUser User { get; set; } = null!;
    public ICollection<Boat> Boats { get; set; } = new List<Boat>();
    public Company? Company { get; set; }
}
