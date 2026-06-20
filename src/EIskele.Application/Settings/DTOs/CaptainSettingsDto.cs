namespace EIskele.Application.Settings.DTOs;

public class CaptainSettingsDto
{
    public CaptainProfileDto Profile { get; set; } = new();
    public CaptainApplicationDto Application { get; set; } = new();
    public CaptainPaymentDto Payment { get; set; } = new();
    public CaptainSecurityDto Security { get; set; } = new();
    public CaptainNotificationsDto Notifications { get; set; } = new();
    public CaptainLegalDto Legal { get; set; } = new();
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
}

public class CaptainPaymentDto
{
    public string BankName { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public string AccountHolderName { get; set; } = string.Empty;
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
    public LegalPermissionsDto Permissions { get; set; } = new();
}

public class LegalPermissionsDto
{
    public bool Kvkk { get; set; }
    public bool Commercial { get; set; }
}
