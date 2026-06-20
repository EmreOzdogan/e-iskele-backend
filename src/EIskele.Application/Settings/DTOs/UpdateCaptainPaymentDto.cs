namespace EIskele.Application.Settings.DTOs;

public class UpdateCaptainPaymentDto
{
    public string BankName { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public string AccountHolderName { get; set; } = string.Empty;
}
