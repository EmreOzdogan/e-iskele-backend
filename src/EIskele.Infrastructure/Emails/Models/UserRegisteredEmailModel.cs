namespace EIskele.Infrastructure.Emails.Models
{
    public class UserRegisteredEmailModel : BaseEmailTemplateModel
    {
        public string UserName { get; set; } = default!;
        public string ActionUrl { get; set; } = default!;
    }
}
