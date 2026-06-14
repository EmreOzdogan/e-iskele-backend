namespace EIskele.Infrastructure.Emails.Models;

public class ReviewRequestEmailModel : BaseEmailTemplateModel
{
    public string CustomerName { get; set; } = string.Empty;
    public string BoatName { get; set; } = string.Empty;
    public string TourDate { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
}
