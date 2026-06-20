using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class UserSecurityEvent : BaseEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;

    // Navigation
    public ApplicationUser User { get; set; } = null!;
}
