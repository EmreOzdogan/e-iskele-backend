using System;

namespace EIskele.Application.Common.Settings;

public class SettingsAuditLogDto
{
    public string Id { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string SettingGroup { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
    public string ActorName { get; set; } = string.Empty;
    public string ActorIp { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "success";
}
