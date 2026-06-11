namespace EIskele.Application.Users.DTOs;

public class UserSecurityInfoDto
{
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
    public int FailedLoginAttempts { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LastPasswordChangedAt { get; set; }
    public int ActiveSessionCount { get; set; }
}
