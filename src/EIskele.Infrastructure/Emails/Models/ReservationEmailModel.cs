namespace EIskele.Infrastructure.Emails.Models;

public class ReservationEmailModel : BaseEmailTemplateModel
{
    public string RecipientName { get; set; } = string.Empty;
    public string BoatName { get; set; } = string.Empty;
    public string PackageName { get; set; } = string.Empty;
    public string ReservationDate { get; set; } = string.Empty;
    public int GuestCount { get; set; }
    public decimal TotalPrice { get; set; }
    public string StatusMessage { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
}
