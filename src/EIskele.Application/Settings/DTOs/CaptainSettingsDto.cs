namespace EIskele.Application.Settings.DTOs;

public class CaptainSettingsDto
{
    public CaptainAccountDto Account { get; set; } = new();
    public CaptainProfileDto Profile { get; set; } = new();
    public CaptainApplicationDto Application { get; set; } = new();
    public CaptainPaymentDto Payment { get; set; } = new();
    public CaptainSecurityDto Security { get; set; } = new();
    public CaptainNotificationsDto Notifications { get; set; } = new();
    public CaptainLegalDto Legal { get; set; } = new();
    public CaptainVerificationSummaryDto VerificationSummary { get; set; } = new();
}

public class CaptainAccountDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    public string Phone { get; set; } = string.Empty;
    public bool PhoneVerified { get; set; }
    public string AccountType { get; set; } = string.Empty;
    public string VerificationStatus { get; set; } = string.Empty;
    public string DocumentStatusText { get; set; } = string.Empty;
    public string PaymentStatusText { get; set; } = string.Empty;
    public string LastUpdatedText { get; set; } = string.Empty;
}

public class CaptainVerificationSummaryDto
{
    public int CompletionRate { get; set; }
    public List<VerificationSummaryItemDto> Items { get; set; } = new();
}

public class VerificationSummaryItemDto
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ActionPath { get; set; }
}

public class CaptainProfileDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ShortBio { get; set; } = string.Empty;
    public string[] Languages { get; set; } = Array.Empty<string>();
}

public class CaptainApplicationDto
{
    public string Status { get; set; } = string.Empty;
    public string SubmittedAt { get; set; } = string.Empty;
    public string DocumentStatus { get; set; } = string.Empty;
    public string VerificationLevel { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string? IdentityNumberMasked { get; set; }
    public string? BirthDate { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? LicenseInfo { get; set; }
    public string? CompanyTitle { get; set; }
    public string? TaxNumber { get; set; }
    public string? TaxOffice { get; set; }
    public string? TradeRegistryNumber { get; set; }
    public string? CompanyAddress { get; set; }
    public string? AuthorizedPerson { get; set; }
    public string ReviewStatus { get; set; } = string.Empty;
    public string? AdminNote { get; set; }
    public string? LastReviewDateText { get; set; }
}

public class CaptainPaymentDto
{
    public string BankName { get; set; } = string.Empty;
    public string IbanMasked { get; set; } = string.Empty;
    public string? IbanRaw { get; set; }
    public string AccountHolderName { get; set; } = string.Empty;
    public string InvoiceType { get; set; } = string.Empty;
    public string VerificationStatus { get; set; } = string.Empty;
    public string? LastUpdatedText { get; set; }
}

public class CaptainSecurityDto
{
    public string LastPasswordChange { get; set; } = string.Empty;
    public bool TwoFactorEnabled { get; set; }
    public List<ActiveSessionDto> ActiveSessions { get; set; } = new();
    public List<SecurityEventDto> RecentEvents { get; set; } = new();
}

public class ActiveSessionDto
{
    public string Id { get; set; } = string.Empty;
    public string Device { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string LastAccess { get; set; } = string.Empty;
    public bool IsCurrent { get; set; }
}

public class SecurityEventDto
{
    public string Id { get; set; } = string.Empty;
    public string Event { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string Device { get; set; } = string.Empty;
}

public class CaptainNotificationsDto
{
    public bool Email { get; set; }
    public bool Sms { get; set; }
    public bool Whatsapp { get; set; }
    public bool Push { get; set; }
    public bool Promotional { get; set; }
}

public class CaptainLegalDto
{
    public string ContractStatus { get; set; } = string.Empty;
    public string ContractDate { get; set; } = string.Empty;
    public List<LegalAgreementDto> Agreements { get; set; } = new();
    public LegalPermissionsDto Permissions { get; set; } = new();
}

public class LegalAgreementDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string? AcceptedAtText { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class LegalPermissionsDto
{
    public bool CommercialEmail { get; set; }
    public bool CampaignSms { get; set; }
    public bool WhatsappInfo { get; set; }
    public bool CookiePreferences { get; set; }
}
