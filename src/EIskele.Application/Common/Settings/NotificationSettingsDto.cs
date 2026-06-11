using System.Collections.Generic;

namespace EIskele.Application.Common.Settings;

public class NotificationScenarioDto
{
    public string Key { get; set; } = "";
    public string Name { get; set; } = "";
    public List<string> Channels { get; set; } = new();
    public string Status { get; set; } = "ready";
}

public class NotificationSettingsDto
{
    public bool EmailNotificationsEnabled { get; set; }
    public bool SmsNotificationsEnabled { get; set; }
    public bool PushNotificationsEnabled { get; set; }
    public bool WhatsAppNotificationsEnabled { get; set; }
    public bool InAppNotificationsEnabled { get; set; }
    
    public List<NotificationScenarioDto> Scenarios { get; set; } = new();
}
