using System;

namespace EIskele.Application.Companies;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string TaxNumber { get; set; } = string.Empty;
    public string TaxOffice { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string AuthorizedPersonName { get; set; } = string.Empty;
    public string? Iban { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateCompanyDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string TaxNumber { get; set; } = string.Empty;
    public string TaxOffice { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string AuthorizedPersonName { get; set; } = string.Empty;
    public string? Iban { get; set; }
}

public class CompanyApplicationRequest
{
    public string CompanyName { get; set; } = string.Empty;
    public string TaxNumber { get; set; } = string.Empty;
    public string TaxOffice { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string AuthorizedPersonName { get; set; } = string.Empty;
    public string? Iban { get; set; }
}
