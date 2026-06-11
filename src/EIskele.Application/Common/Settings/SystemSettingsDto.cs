namespace EIskele.Application.Common.Settings;

public class SystemSettingsDto
{
    public string PlatformName { get; set; } = "e-iskele";
    public string? Slogan { get; set; }
    public string DefaultLanguage { get; set; } = "tr";
    public string DefaultCurrency { get; set; } = "TRY";
    public string Timezone { get; set; } = "Europe/Istanbul";

    public string CustomerWebUrl { get; set; } = "";
    public string CaptainHubUrl { get; set; } = "";
    public string AdminPanelUrl { get; set; } = "";
    public string ApiBaseUrl { get; set; } = "";
    public string? CdnUrl { get; set; }

    public string SupportEmail { get; set; } = "";
    public string? SupportPhone { get; set; }
}
