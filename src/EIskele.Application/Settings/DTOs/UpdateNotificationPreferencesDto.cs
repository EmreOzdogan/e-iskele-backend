namespace EIskele.Application.Settings.DTOs;

public class UpdateNotificationPreferencesDto
{
    public bool Email { get; set; }
    public bool Sms { get; set; }
    public bool Whatsapp { get; set; }
    public bool Push { get; set; }
    public bool Promotional { get; set; }
}
