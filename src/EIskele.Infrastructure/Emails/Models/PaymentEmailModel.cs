namespace EIskele.Infrastructure.Emails.Models;

public class PaymentEmailModel : BaseEmailTemplateModel
{
    public string CustomerName { get; set; } = string.Empty;
    public string ReservationId { get; set; } = string.Empty;
    public string BoatName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string ActionUrl { get; set; } = string.Empty;
}
