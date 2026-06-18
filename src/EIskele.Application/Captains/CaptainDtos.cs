using System;
using System.Collections.Generic;

namespace EIskele.Application.Captains;

public class CaptainApplicationRequest
{
    public string ApplicationType { get; set; } = string.Empty; // "individual" or "company"
    
    public ApplicationIndividualDto? Individual { get; set; }
    public ApplicationCompanyDto? Company { get; set; }
    public ApplicationBoatDto Boat { get; set; } = new();
    public ApplicationPayoutDto Payout { get; set; } = new();
    public Dictionary<string, Guid> Documents { get; set; } = new();
}

public class ApplicationIndividualDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? BirthDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? IdentityNumber { get; set; }
}

public class ApplicationCompanyDto
{
    public string CompanyTitle { get; set; } = string.Empty;
    public string AuthorizedPersonFullName { get; set; } = string.Empty;
    public string TaxOffice { get; set; } = string.Empty;
    public string TaxNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class ApplicationBoatDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? BrandModel { get; set; }
    public string? ProductionYear { get; set; }
    public string? Length { get; set; }
    public int Capacity { get; set; }
    public Guid LocationId { get; set; }
    public Guid HarborId { get; set; }
    public string? Description { get; set; }
    public List<string> Features { get; set; } = new();
}

public class ApplicationPayoutDto
{
    public string Iban { get; set; } = string.Empty;
    public string AccountHolderName { get; set; } = string.Empty;
    public string? BankName { get; set; }
}

public class CaptainApplicationResponse
{
    public Guid ApplicationId { get; set; }
    public string ApplicationNo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
