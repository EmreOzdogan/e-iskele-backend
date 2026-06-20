using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class UserActiveSession : BaseEntity
{
    public Guid UserId { get; set; }
    public string Device { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime LastAccess { get; set; }
    public bool IsCurrent { get; set; }

    // Navigation
    public ApplicationUser User { get; set; } = null!;
}
