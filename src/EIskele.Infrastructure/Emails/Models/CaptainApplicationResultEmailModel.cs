namespace EIskele.Infrastructure.Emails.Models;

public class CaptainApplicationResultEmailModel : BaseEmailTemplateModel
{
    public string CaptainName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string SupportUrl { get; set; } = string.Empty;
}
