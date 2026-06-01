using System;
using System.Collections.Generic;

namespace EIskele.Application.Common.Notifications;

public class NotificationRequest
{
    public Guid UserId { get; set; }
    public string TemplateCode { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = new();
}
