using EIskele.Domain.Common;
using EIskele.Domain.Enums;

namespace EIskele.Domain.Entities;

public class PackageInclude : BaseEntity
{
    public Guid TourPackageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsIncluded { get; set; }
    public string? Description { get; set; }
    public PackageIncludeStatus Status { get; set; } = PackageIncludeStatus.Active;

    // Navigation
    public TourPackage TourPackage { get; set; } = null!;
}
