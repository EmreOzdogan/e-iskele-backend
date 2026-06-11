using System;

namespace EIskele.Application.Common.Settings;

public class SmsSettingsDto
{
    public bool SmsEnabled { get; set; } = false;
    public string SmsProvider { get; set; } = "none";
    public string SmsSenderTitle { get; set; } = "EISKELE";
}
