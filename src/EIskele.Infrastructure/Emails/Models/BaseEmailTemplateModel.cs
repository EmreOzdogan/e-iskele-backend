using System;

namespace EIskele.Infrastructure.Emails.Models;

public abstract class BaseEmailTemplateModel
{
    public string AppName { get; set; } = "e-iskele";
    public string BaseUrl { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string SupportEmail { get; set; } = string.Empty;
    public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.UtcNow;
    public int CurrentYear { get; set; } = DateTime.UtcNow.Year;
}
