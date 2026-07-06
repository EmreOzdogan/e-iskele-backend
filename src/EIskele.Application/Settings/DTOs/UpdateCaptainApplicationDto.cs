using System;

namespace EIskele.Application.Settings.DTOs;

public class UpdateCaptainApplicationDto
{
    public DateTime? BirthDate { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? CompanyTitle { get; set; }
    public string? TaxNumber { get; set; }
    public string? TaxOffice { get; set; }
    public string? TradeRegistryNumber { get; set; }
}
