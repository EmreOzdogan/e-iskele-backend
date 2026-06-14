namespace EIskele.Infrastructure.Emails.Models;

public class BoatStatusEmailModel : BaseEmailTemplateModel
{
    public string CaptainName { get; set; } = string.Empty;
    public string BoatName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
}
