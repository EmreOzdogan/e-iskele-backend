using System;

namespace EIskele.Application.Common.Settings;

public class SmtpEmailSettingsDto
{
    public bool SmtpEnabled { get; set; }
    public string SmtpProvider { get; set; } = "basic"; // basic, gmail, m365
    public string SmtpHost { get; set; } = string.Empty;
    public int? SmtpPort { get; set; }
    public string SmtpSecurityType { get; set; } = "none"; // none, sslTls, startTls
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public string M365ClientId { get; set; } = string.Empty;
    public string M365TenantId { get; set; } = string.Empty;
    public string M365ClientSecret { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string ReplyToEmail { get; set; } = string.Empty;
}
