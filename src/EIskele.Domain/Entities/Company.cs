using System;
using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class Company : SoftDeletableEntity
{
    public Guid CaptainId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string AuthorizedPersonName { get; set; } = string.Empty;
    public string TaxOffice { get; set; } = string.Empty;
    public string TaxNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Iban { get; set; }

    // Navigation
    public Captain Captain { get; set; } = null!;
}
