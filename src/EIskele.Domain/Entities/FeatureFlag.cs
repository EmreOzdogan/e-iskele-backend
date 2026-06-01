using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class FeatureFlag : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = false;
    public string? Description { get; set; }
    public string Group { get; set; } = string.Empty;
}
