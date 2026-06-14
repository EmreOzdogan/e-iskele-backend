namespace EIskele.Infrastructure.Emails.Models;

public class CaptainApplicationReceivedEmailModel : BaseEmailTemplateModel
{
    public string CaptainName { get; set; } = string.Empty;
    public string ApplicationDate { get; set; } = string.Empty;
    public string SupportUrl { get; set; } = string.Empty;
}
