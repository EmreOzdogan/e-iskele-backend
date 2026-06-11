using System;

namespace EIskele.Application.Common.Settings;

public class SecuritySettingsDto
{
    public int AdminSessionMinutes { get; set; } = 60;
    public int RefreshTokenDays { get; set; } = 7;
    public int MaxFailedLoginAttempts { get; set; } = 5;
    public int AccountLockoutMinutes { get; set; } = 15;
    public int PasswordMinimumLength { get; set; } = 8;
    public bool RequirePasswordComplexity { get; set; } = true;
    public bool AuditLogEnabled { get; set; } = true;
}
