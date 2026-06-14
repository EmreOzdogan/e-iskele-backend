using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class PackageInclude : BaseEntity
{
    public Guid TourPackageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsIncluded { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;

    // Navigation
    public TourPackage TourPackage { get; set; } = null!;
}
