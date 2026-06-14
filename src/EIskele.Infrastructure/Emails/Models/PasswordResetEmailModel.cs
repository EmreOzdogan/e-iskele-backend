namespace EIskele.Infrastructure.Emails.Models;

public class PasswordResetEmailModel : BaseEmailTemplateModel
{
    public string UserName { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
}
