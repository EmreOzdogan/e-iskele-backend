using System;

namespace EIskele.Application.Captains;

public class CaptainApplicationRequest
{
    public Guid UserId { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public string ExperienceYears { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class CaptainApplicationResponse
{
    public Guid ApplicationId { get; set; }
    public string Status { get; set; } = string.Empty;
}
