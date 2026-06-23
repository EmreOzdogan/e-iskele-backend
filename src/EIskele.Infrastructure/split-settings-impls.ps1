$infraSettingsDir = "C:\Users\Ozem\Desktop\Projeler\e-iskele Projesi\eiskele\backend\src\EIskele.Infrastructure\Settings"

# 1. SystemSettingsProvider
$providerImpl = @"
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Settings;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Settings;

public class SystemSettingsProvider : ISystemSettingsProvider
{
    private readonly EIskeleDbContext _dbContext;

    public SystemSettingsProvider(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GetSettingValueAsync(string key, string defaultValue = "", CancellationToken cancellationToken = default)
    {
        var setting = await _dbContext.SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);

        return setting?.Value ?? defaultValue;
    }

    public async Task<T> GetSettingValueAsync<T>(string key, T defaultValue, CancellationToken cancellationToken = default)
    {
        var stringValue = await GetSettingValueAsync(key, string.Empty, cancellationToken);

        if (string.IsNullOrEmpty(stringValue))
            return defaultValue;

        try
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null && converter.CanConvertFrom(typeof(string)))
            {
                var convertedValue = converter.ConvertFromString(stringValue);
                return convertedValue == null ? defaultValue : (T)convertedValue;
            }

            return (T)Convert.ChangeType(stringValue, typeof(T));
        }
        catch
        {
            return defaultValue;
        }
    }

    public async Task<bool> IsFeatureEnabledAsync(string key, bool defaultValue = false, CancellationToken cancellationToken = default)
    {
        var feature = await _dbContext.FeatureFlags
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Key == key, cancellationToken);

        return feature?.IsEnabled ?? defaultValue;
    }
}
"@
Set-Content -Path "$infraSettingsDir\SystemSettingsProvider.cs" -Value $providerImpl -Encoding UTF8

# 2. GeneralSettingsService
$generalImpl = @"
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Common.Settings;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Settings;

public class GeneralSettingsService : IGeneralSettingsService
{
    private readonly EIskeleDbContext _dbContext;

    public GeneralSettingsService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<SystemSettingsDto>> GetGeneralSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
        string GetValue(string key, string def) => settings.FirstOrDefault(s => s.Key == key)?.Value ?? def;

        var dto = new SystemSettingsDto
        {
            PlatformName = GetValue("General.PlatformName", "e-iskele"),
            Slogan = GetValue("General.Slogan", ""),
            DefaultLanguage = GetValue("General.DefaultLanguage", "tr"),
            DefaultCurrency = GetValue("General.DefaultCurrency", "TRY"),
            Timezone = GetValue("General.Timezone", "Europe/Istanbul"),
            CustomerWebUrl = GetValue("General.CustomerWebUrl", ""),
            CaptainHubUrl = GetValue("General.CaptainHubUrl", ""),
            AdminPanelUrl = GetValue("General.AdminPanelUrl", ""),
            ApiBaseUrl = GetValue("General.ApiBaseUrl", ""),
            CdnUrl = GetValue("General.CdnUrl", ""),
            SupportEmail = GetValue("General.SupportEmail", ""),
            SupportPhone = GetValue("General.SupportPhone", "")
        };

        return Result<SystemSettingsDto>.Success(dto);
    }

    public async Task<Result> UpdateGeneralSettingsAsync(SystemSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.ToListAsync(cancellationToken);

        void UpdateValue(string key, string value)
        {
            var setting = settings.FirstOrDefault(s => s.Key == key);
            if (setting == null)
            {
                if (value != null)
                {
                    _dbContext.SystemSettings.Add(new Domain.Entities.SystemSetting
                    {
                        Key = key,
                        Value = value,
                        ValueType = SystemSettingValueType.String,
                        Group = "General",
                        Description = "",
                        IsEditable = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = currentUserId
                    });
                }
            }
            else
            {
                if (setting.Value != value)
                {
                    setting.Value = value ?? "";
                    setting.UpdatedAt = DateTime.UtcNow;
                    setting.UpdatedBy = currentUserId;
                }
            }
        }

        UpdateValue("General.PlatformName", dto.PlatformName);
        UpdateValue("General.Slogan", dto.Slogan);
        UpdateValue("General.DefaultLanguage", dto.DefaultLanguage);
        UpdateValue("General.DefaultCurrency", dto.DefaultCurrency);
        UpdateValue("General.Timezone", dto.Timezone);
        UpdateValue("General.CustomerWebUrl", dto.CustomerWebUrl);
        UpdateValue("General.CaptainHubUrl", dto.CaptainHubUrl);
        UpdateValue("General.AdminPanelUrl", dto.AdminPanelUrl);
        UpdateValue("General.ApiBaseUrl", dto.ApiBaseUrl);
        UpdateValue("General.CdnUrl", dto.CdnUrl);
        UpdateValue("General.SupportEmail", dto.SupportEmail);
        UpdateValue("General.SupportPhone", dto.SupportPhone);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
"@
Set-Content -Path "$infraSettingsDir\GeneralSettingsService.cs" -Value $generalImpl -Encoding UTF8

# 3. ReservationRulesSettingsService
$reservationImpl = @"
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Common.Settings;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Settings;

public class ReservationRulesSettingsService : IReservationRulesSettingsService
{
    private readonly EIskeleDbContext _dbContext;

    public ReservationRulesSettingsService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<ReservationRulesSettingsDto>> GetReservationRulesSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
        
        string GetValueStr(string key, string def) => settings.FirstOrDefault(s => s.Key == key)?.Value ?? def;
        int GetValueInt(string key, int def) => int.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;
        bool GetValueBool(string key, bool def) => bool.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;

        var dto = new ReservationRulesSettingsDto
        {
            DefaultReservationApprovalType = GetValueStr("Reservation.DefaultReservationApprovalType", "captainApproval"),
            CaptainApprovalTimeoutHours = GetValueInt("Reservation.CaptainApprovalTimeoutHours", 12),
            MinimumReservationLeadTimeHours = GetValueInt("Reservation.MinimumReservationLeadTimeHours", 24),
            MaxAdvanceReservationDays = GetValueInt("Reservation.MaxAdvanceReservationDays", 90),
            CancellationAllowedHoursBeforeTour = GetValueInt("Reservation.CancellationAllowedHoursBeforeTour", 48),
            WeatherPostponeEnabled = GetValueBool("Reservation.WeatherPostponeEnabled", true),
            GuestCountValidationEnabled = GetValueBool("Reservation.GuestCountValidationEnabled", true),
            PreventOverlappingReservations = GetValueBool("Reservation.PreventOverlappingReservations", true)
        };

        return Result<ReservationRulesSettingsDto>.Success(dto);
    }

    public async Task<Result> UpdateReservationRulesSettingsAsync(ReservationRulesSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.ToListAsync(cancellationToken);

        void UpdateValue(string key, string value, string valueType)
        {
            var setting = settings.FirstOrDefault(s => s.Key == key);
            if (setting == null)
            {
                _dbContext.SystemSettings.Add(new Domain.Entities.SystemSetting
                {
                    Key = key,
                    Value = value,
                    ValueType = Enum.Parse<SystemSettingValueType>(valueType, true),
                    Group = "ReservationRules",
                    Description = "",
                    IsEditable = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId
                });
            }
            else if (setting.Value != value)
            {
                setting.Value = value;
                setting.UpdatedAt = DateTime.UtcNow;
                setting.UpdatedBy = currentUserId;
            }
        }

        UpdateValue("Reservation.DefaultReservationApprovalType", dto.DefaultReservationApprovalType, "String");
        UpdateValue("Reservation.CaptainApprovalTimeoutHours", dto.CaptainApprovalTimeoutHours.ToString(), "Int");
        UpdateValue("Reservation.MinimumReservationLeadTimeHours", dto.MinimumReservationLeadTimeHours.ToString(), "Int");
        UpdateValue("Reservation.MaxAdvanceReservationDays", dto.MaxAdvanceReservationDays.ToString(), "Int");
        UpdateValue("Reservation.CancellationAllowedHoursBeforeTour", dto.CancellationAllowedHoursBeforeTour.ToString(), "Int");
        UpdateValue("Reservation.WeatherPostponeEnabled", dto.WeatherPostponeEnabled.ToString(), "Boolean");
        UpdateValue("Reservation.GuestCountValidationEnabled", dto.GuestCountValidationEnabled.ToString(), "Boolean");
        UpdateValue("Reservation.PreventOverlappingReservations", dto.PreventOverlappingReservations.ToString(), "Boolean");

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
"@
Set-Content -Path "$infraSettingsDir\ReservationRulesSettingsService.cs" -Value $reservationImpl -Encoding UTF8

# 4. CommissionFinanceSettingsService
$financeImpl = @"
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Common.Settings;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Settings;

public class CommissionFinanceSettingsService : ICommissionFinanceSettingsService
{
    private readonly EIskeleDbContext _dbContext;

    public CommissionFinanceSettingsService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<CommissionFinanceSettingsDto>> GetCommissionFinanceSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
        
        string GetValueStr(string key, string def) => settings.FirstOrDefault(s => s.Key == key)?.Value ?? def;
        int GetValueInt(string key, int def) => int.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;
        decimal GetValueDecimal(string key, decimal def) => decimal.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;

        var dto = new CommissionFinanceSettingsDto
        {
            PlatformCommissionRate = GetValueDecimal("Finance.PlatformCommissionRate", 10m),
            ServiceFeeType = GetValueStr("Finance.ServiceFeeType", "percentage"),
            ServiceFeeRate = GetValueDecimal("Finance.ServiceFeeRate", 5m),
            ServiceFeeFixedAmount = GetValueDecimal("Finance.ServiceFeeFixedAmount", 0m),
            MinimumDepositRate = GetValueDecimal("Finance.MinimumDepositRate", 30m),
            MaximumDepositRate = GetValueDecimal("Finance.MaximumDepositRate", 100m),
            PayoutHoldDays = GetValueInt("Finance.PayoutHoldDays", 3),
            RefundReviewDays = GetValueInt("Finance.RefundReviewDays", 7),
            CaptainPayoutPeriod = GetValueStr("Finance.CaptainPayoutPeriod", "weekly")
        };

        return Result<CommissionFinanceSettingsDto>.Success(dto);
    }

    public async Task<Result> UpdateCommissionFinanceSettingsAsync(CommissionFinanceSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.ToListAsync(cancellationToken);

        void UpdateValue(string key, string value, string valueType)
        {
            var setting = settings.FirstOrDefault(s => s.Key == key);
            if (setting == null)
            {
                _dbContext.SystemSettings.Add(new Domain.Entities.SystemSetting
                {
                    Key = key,
                    Value = value,
                    ValueType = Enum.Parse<SystemSettingValueType>(valueType, true),
                    Group = "CommissionFinance",
                    Description = "",
                    IsEditable = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId
                });
            }
            else if (setting.Value != value)
            {
                setting.Value = value;
                setting.UpdatedAt = DateTime.UtcNow;
                setting.UpdatedBy = currentUserId;
            }
        }

        UpdateValue("Finance.PlatformCommissionRate", dto.PlatformCommissionRate.ToString(), "Decimal");
        UpdateValue("Finance.ServiceFeeType", dto.ServiceFeeType, "String");
        UpdateValue("Finance.ServiceFeeRate", (dto.ServiceFeeRate ?? 0).ToString(), "Decimal");
        UpdateValue("Finance.ServiceFeeFixedAmount", (dto.ServiceFeeFixedAmount ?? 0).ToString(), "Decimal");
        UpdateValue("Finance.MinimumDepositRate", dto.MinimumDepositRate.ToString(), "Decimal");
        UpdateValue("Finance.MaximumDepositRate", dto.MaximumDepositRate.ToString(), "Decimal");
        UpdateValue("Finance.PayoutHoldDays", dto.PayoutHoldDays.ToString(), "Int");
        UpdateValue("Finance.RefundReviewDays", dto.RefundReviewDays.ToString(), "Int");
        UpdateValue("Finance.CaptainPayoutPeriod", dto.CaptainPayoutPeriod, "String");

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
"@
Set-Content -Path "$infraSettingsDir\CommissionFinanceSettingsService.cs" -Value $financeImpl -Encoding UTF8

# 5. SmtpEmailSettingsService
$emailImpl = @"
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Common.Settings;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Identity.Client;

namespace EIskele.Infrastructure.Settings;

public class SmtpEmailSettingsService : ISmtpEmailSettingsService
{
    private readonly EIskeleDbContext _dbContext;
    private readonly EIskele.Infrastructure.Emails.Services.IEmailTemplateRenderer _emailTemplateRenderer;
    private readonly EIskele.Infrastructure.Emails.Services.IEmailSender _emailSender;

    public SmtpEmailSettingsService(
        EIskeleDbContext dbContext,
        EIskele.Infrastructure.Emails.Services.IEmailTemplateRenderer emailTemplateRenderer,
        EIskele.Infrastructure.Emails.Services.IEmailSender emailSender)
    {
        _dbContext = dbContext;
        _emailTemplateRenderer = emailTemplateRenderer;
        _emailSender = emailSender;
    }

    public async Task<Result<SmtpEmailSettingsDto>> GetSmtpEmailSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
        
        string GetValueStr(string key, string def) => settings.FirstOrDefault(s => s.Key == key)?.Value ?? def;
        int GetValueInt(string key, int def) => int.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;
        bool GetValueBool(string key, bool def) => bool.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;

        var dto = new SmtpEmailSettingsDto
        {
            SmtpEnabled = GetValueBool("Smtp.SmtpEnabled", false),
            SmtpProvider = GetValueStr("Smtp.SmtpProvider", "basic"),
            SmtpHost = GetValueStr("Smtp.SmtpHost", ""),
            SmtpPort = GetValueInt("Smtp.SmtpPort", 587),
            SmtpSecurityType = GetValueStr("Smtp.SmtpSecurityType", "none"),
            SmtpUsername = GetValueStr("Smtp.SmtpUsername", ""),
            SmtpPassword = GetValueStr("Smtp.SmtpPassword", ""),
            SenderEmail = GetValueStr("Smtp.SenderEmail", ""),
            SenderName = GetValueStr("Smtp.SenderName", ""),
            ReplyToEmail = GetValueStr("Smtp.ReplyToEmail", ""),
            M365ClientId = GetValueStr("Smtp.M365ClientId", ""),
            M365TenantId = GetValueStr("Smtp.M365TenantId", ""),
            M365ClientSecret = GetValueStr("Smtp.M365ClientSecret", "")
        };

        return Result<SmtpEmailSettingsDto>.Success(dto);
    }

    public async Task<Result> UpdateSmtpEmailSettingsAsync(SmtpEmailSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.ToListAsync(cancellationToken);

        void UpdateValue(string key, string value, string valueType)
        {
            var setting = settings.FirstOrDefault(s => s.Key == key);
            if (setting == null)
            {
                _dbContext.SystemSettings.Add(new Domain.Entities.SystemSetting
                {
                    Key = key,
                    Value = value,
                    ValueType = Enum.Parse<SystemSettingValueType>(valueType, true),
                    Group = "SmtpEmail",
                    Description = "",
                    IsEditable = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId
                });
            }
            else if (setting.Value != value)
            {
                setting.Value = value;
                setting.UpdatedAt = DateTime.UtcNow;
                setting.UpdatedBy = currentUserId;
            }
        }

        UpdateValue("Smtp.SmtpEnabled", dto.SmtpEnabled.ToString(), "Boolean");
        UpdateValue("Smtp.SmtpProvider", dto.SmtpProvider, "String");
        UpdateValue("Smtp.SmtpHost", dto.SmtpHost ?? "", "String");
        UpdateValue("Smtp.SmtpPort", (dto.SmtpPort ?? 587).ToString(), "Int");
        UpdateValue("Smtp.SmtpSecurityType", dto.SmtpSecurityType ?? "none", "String");
        UpdateValue("Smtp.SmtpUsername", dto.SmtpUsername ?? "", "String");
        UpdateValue("Smtp.SmtpPassword", dto.SmtpPassword ?? "", "String");
        UpdateValue("Smtp.SenderEmail", dto.SenderEmail ?? "", "String");
        UpdateValue("Smtp.SenderName", dto.SenderName ?? "", "String");
        UpdateValue("Smtp.ReplyToEmail", dto.ReplyToEmail ?? "", "String");
        UpdateValue("Smtp.M365ClientId", dto.M365ClientId ?? "", "String");
        UpdateValue("Smtp.M365TenantId", dto.M365TenantId ?? "", "String");
        UpdateValue("Smtp.M365ClientSecret", dto.M365ClientSecret ?? "", "String");

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private SecureSocketOptions GetSecureSocketOption(string securityType)
    {
        return securityType?.ToLower() switch
        {
            "ssltls" => SecureSocketOptions.SslOnConnect,
            "starttls" => SecureSocketOptions.StartTls,
            _ => SecureSocketOptions.Auto
        };
    }

    private async Task AuthenticateSmtpClientAsync(SmtpClient client, SmtpEmailSettingsDto dto, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(dto.SmtpUsername) && !string.IsNullOrEmpty(dto.SmtpPassword))
        {
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            await client.AuthenticateAsync(dto.SmtpUsername, dto.SmtpPassword, cancellationToken);
        }
    }

    public async Task<Result> TestSmtpConnectionAsync(SmtpEmailSettingsDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (dto.SmtpProvider == "m365" && !string.IsNullOrEmpty(dto.M365ClientId) && !string.IsNullOrEmpty(dto.M365TenantId) && !string.IsNullOrEmpty(dto.M365ClientSecret))
            {
                var cca = ConfidentialClientApplicationBuilder.Create(dto.M365ClientId)
                    .WithAuthority($"https://login.microsoftonline.com/{dto.M365TenantId}/v2.0")
                    .WithClientSecret(dto.M365ClientSecret)
                    .Build();

                var scopes = new[] { "https://graph.microsoft.com/.default" };
                var authResult = await cca.AcquireTokenForClient(scopes).ExecuteAsync(cancellationToken);
                
                if (!string.IsNullOrEmpty(authResult.AccessToken))
                    return Result.Success();
                    
                return Result.Failure("M365AuthError", "Microsoft 365 token alınamadı.");
            }

            using var client = new SmtpClient();
            var secureOption = GetSecureSocketOption(dto.SmtpSecurityType);

            await client.ConnectAsync(dto.SmtpHost, dto.SmtpPort ?? 587, secureOption, cancellationToken);
            await AuthenticateSmtpClientAsync(client, dto, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure("SmtpConnectionError", ex.Message);
        }
    }

    public async Task<Result> SendTestEmailAsync(string email, SmtpEmailSettingsDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = new EIskele.Infrastructure.Emails.Models.TestEmailModel
            {
                BaseUrl = "https://www.e-iskele.com",
                AdminPanelUrl = "https://admin.e-iskele.com"
            };

            var htmlBody = await _emailTemplateRenderer.RenderAsync("TestEmail", model, cancellationToken);

            var assembly = typeof(SmtpEmailSettingsService).Assembly;
            using var logoStream = assembly.GetManifestResourceStream("EIskele.Infrastructure.Resources.e-iskele_logo.png");
            byte[]? logoBytes = null;
            if (logoStream != null)
            {
                using var ms = new System.IO.MemoryStream();
                await logoStream.CopyToAsync(ms, cancellationToken);
                logoBytes = ms.ToArray();
            }

            await _emailSender.SendAsync(email, "e-iskele SMTP Test E-postası", htmlBody, dto, logoBytes, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure("SmtpSendError", ex.Message);
        }
    }

    public async Task<Result> SendTestScenarioEmailAsync(string scenarioKey, string email, SmtpEmailSettingsDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var htmlBody = string.Empty;
            string subject = "Test Bildirimi";

            if (scenarioKey == "user_registered")
            {
                var model = new EIskele.Infrastructure.Emails.Models.UserRegisteredEmailModel
                {
                    BaseUrl = "https://www.e-iskele.com",
                    ActionUrl = "https://www.e-iskele.com/login",
                    UserName = "Test Kullanıcısı"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("UserRegistered", model, cancellationToken);
                subject = "e-iskele: Hoş Geldiniz";
            }
            else if (scenarioKey == "password_reset")
            {
                var model = new EIskele.Infrastructure.Emails.Models.PasswordResetEmailModel
                {
                    UserName = "Test Kullanıcısı",
                    ActionUrl = "https://www.e-iskele.com/sifre-sifirla?token=test"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("PasswordReset", model, cancellationToken);
                subject = "e-iskele: Şifre Sıfırlama Talebi";
            }
            else if (scenarioKey == "captain_application_received")
            {
                var model = new EIskele.Infrastructure.Emails.Models.CaptainApplicationReceivedEmailModel
                {
                    CaptainName = "Örnek Kaptan",
                    ApplicationDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                    SupportUrl = "https://kaptanhub.e-iskele.com/destek"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("CaptainApplicationReceived", model, cancellationToken);
                subject = "e-iskele: Kaptan Başvurunuz Alındı";
            }
            else if (scenarioKey == "captain_application_approved")
            {
                var model = new EIskele.Infrastructure.Emails.Models.CaptainApplicationResultEmailModel
                {
                    CaptainName = "Örnek Kaptan",
                    SupportUrl = "https://kaptanhub.e-iskele.com/"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("CaptainApplicationApproved", model, cancellationToken);
                subject = "e-iskele: Kaptan Başvurunuz Onaylandı";
            }
            else if (scenarioKey == "captain_application_rejected")
            {
                var model = new EIskele.Infrastructure.Emails.Models.CaptainApplicationResultEmailModel
                {
                    CaptainName = "Örnek Kaptan",
                    Reason = "Belgeleriniz doğrulanamadı. Lütfen geçerli bir ticari ruhsat yükleyiniz.",
                    SupportUrl = "https://kaptanhub.e-iskele.com/destek"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("CaptainApplicationRejected", model, cancellationToken);
                subject = "e-iskele: Kaptan Başvurunuz Reddedildi";
            }
            else if (scenarioKey == "captain_application_missing_document")
            {
                var model = new EIskele.Infrastructure.Emails.Models.CaptainApplicationResultEmailModel
                {
                    CaptainName = "Örnek Kaptan",
                    Reason = "Kimlik fotokopiniz okunaksızdır. Lütfen daha net bir fotoğraf yükleyiniz.",
                    SupportUrl = "https://kaptanhub.e-iskele.com/belgeler"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("CaptainApplicationMissingDocument", model, cancellationToken);
                subject = "e-iskele: Eksik Belge Talebi";
            }
            else if (scenarioKey == "boat_published")
            {
                var model = new EIskele.Infrastructure.Emails.Models.BoatStatusEmailModel
                {
                    CaptainName = "Örnek Kaptan",
                    BoatName = "Mavi Rüya",
                    ActionUrl = "https://kaptanhub.e-iskele.com/tekneler"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("BoatPublished", model, cancellationToken);
                subject = "e-iskele: Tekneniz Yayına Alındı";
            }
            else if (scenarioKey == "boat_rejected")
            {
                var model = new EIskele.Infrastructure.Emails.Models.BoatStatusEmailModel
                {
                    CaptainName = "Örnek Kaptan",
                    BoatName = "Mavi Rüya",
                    Reason = "Tekne görselleri kalite standartlarımıza uymamaktadır. Lütfen daha yüksek çözünürlüklü görseller ekleyiniz.",
                    ActionUrl = "https://kaptanhub.e-iskele.com/tekneler"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("BoatRejected", model, cancellationToken);
                subject = "e-iskele: Tekne Yayına Alımı Reddedildi";
            }
            else if (scenarioKey == "reservation_created_customer")
            {
                var model = new EIskele.Infrastructure.Emails.Models.ReservationEmailModel
                {
                    RecipientName = "Örnek Müşteri",
                    BoatName = "Mavi Rüya",
                    PackageName = "Tam Gün Balık Turu",
                    ReservationDate = DateTime.Now.AddDays(7).ToString("dd.MM.yyyy HH:mm"),
                    GuestCount = 4,
                    TotalPrice = 5000.00m,
                    StatusMessage = "Rezervasyon talebiniz kaptana iletildi. Kaptan onayladığında ödeme adımına geçebileceksiniz.",
                    ActionUrl = "https://www.e-iskele.com/hesabim/rezervasyonlar"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("ReservationCreatedCustomer", model, cancellationToken);
                subject = "e-iskele: Rezervasyon Talebiniz Alındı";
            }
            else if (scenarioKey == "reservation_created_captain")
            {
                var model = new EIskele.Infrastructure.Emails.Models.ReservationEmailModel
                {
                    RecipientName = "Örnek Kaptan",
                    BoatName = "Mavi Rüya",
                    PackageName = "Tam Gün Balık Turu",
                    ReservationDate = DateTime.Now.AddDays(7).ToString("dd.MM.yyyy HH:mm"),
                    GuestCount = 4,
                    TotalPrice = 5000.00m,
                    StatusMessage = "Lütfen 24 saat içerisinde talebi onaylayın veya reddedin.",
                    ActionUrl = "https://kaptanhub.e-iskele.com/rezervasyonlar"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("ReservationCreatedCaptain", model, cancellationToken);
                subject = "e-iskele: Yeni Rezervasyon Talebi Var";
            }
            else if (scenarioKey == "reservation_approved")
            {
                var model = new EIskele.Infrastructure.Emails.Models.ReservationEmailModel
                {
                    RecipientName = "Örnek Müşteri",
                    BoatName = "Mavi Rüya",
                    ReservationDate = DateTime.Now.AddDays(7).ToString("dd.MM.yyyy HH:mm"),
                    TotalPrice = 5000.00m,
                    StatusMessage = "Talebiniz kaptan tarafından onaylandı! Lütfen ödemenizi tamamlayarak rezervasyonunuzu kesinleştirin.",
                    ActionUrl = "https://www.e-iskele.com/hesabim/rezervasyonlar"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("ReservationApproved", model, cancellationToken);
                subject = "e-iskele: Rezervasyonunuz Onaylandı";
            }
            else if (scenarioKey == "reservation_rejected")
            {
                var model = new EIskele.Infrastructure.Emails.Models.ReservationEmailModel
                {
                    RecipientName = "Örnek Müşteri",
                    BoatName = "Mavi Rüya",
                    ReservationDate = DateTime.Now.AddDays(7).ToString("dd.MM.yyyy HH:mm"),
                    StatusMessage = "Kaptanımız belirttiğiniz tarihlerde teknenin müsait olmadığını bildirmiştir.",
                    ActionUrl = "https://www.e-iskele.com/turlar"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("ReservationRejected", model, cancellationToken);
                subject = "e-iskele: Rezervasyon Talebiniz Onaylanamadı";
            }
            else if (scenarioKey == "reservation_cancelled")
            {
                var model = new EIskele.Infrastructure.Emails.Models.ReservationEmailModel
                {
                    RecipientName = "Örnek Kullanıcı",
                    BoatName = "Mavi Rüya",
                    ReservationDate = DateTime.Now.AddDays(7).ToString("dd.MM.yyyy HH:mm"),
                    StatusMessage = "İptal talebiniz işleme alınmış olup iptal politikasına göre süreç başlatılmıştır.",
                    ActionUrl = "https://www.e-iskele.com/hesabim/rezervasyonlar"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("ReservationCancelled", model, cancellationToken);
                subject = "e-iskele: Rezervasyonunuz İptal Edildi";
            }
            else if (scenarioKey == "reservation_reminder")
            {
                var model = new EIskele.Infrastructure.Emails.Models.ReservationEmailModel
                {
                    RecipientName = "Örnek Müşteri",
                    BoatName = "Mavi Rüya",
                    PackageName = "Tam Gün Balık Turu",
                    ReservationDate = DateTime.Now.AddDays(1).ToString("dd.MM.yyyy HH:mm"),
                    StatusMessage = "Harika bir deniz deneyimi yaşamanız için kaptanımız sizi bekliyor olacak.",
                    ActionUrl = "https://www.e-iskele.com/hesabim/rezervasyonlar"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("ReservationReminder", model, cancellationToken);
                subject = "e-iskele: Turunuz Yaklaşıyor!";
            }
            else if (scenarioKey == "payment_success")
            {
                var model = new EIskele.Infrastructure.Emails.Models.PaymentEmailModel
                {
                    CustomerName = "Örnek Müşteri",
                    ReservationId = "RES-98765432",
                    BoatName = "Mavi Rüya",
                    Amount = 5000.00m,
                    ActionUrl = "https://www.e-iskele.com/hesabim/rezervasyonlar"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("PaymentSuccess", model, cancellationToken);
                subject = "e-iskele: Ödemeniz Başarıyla Alındı";
            }
            else if (scenarioKey == "payment_failed")
            {
                var model = new EIskele.Infrastructure.Emails.Models.PaymentEmailModel
                {
                    CustomerName = "Örnek Müşteri",
                    ReservationId = "RES-98765432",
                    BoatName = "Mavi Rüya",
                    Amount = 5000.00m,
                    ActionUrl = "https://www.e-iskele.com/hesabim/rezervasyonlar"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("PaymentFailed", model, cancellationToken);
                subject = "e-iskele: Ödeme Başarısız Oldu";
            }
            else if (scenarioKey == "review_request")
            {
                var model = new EIskele.Infrastructure.Emails.Models.ReviewRequestEmailModel
                {
                    CustomerName = "Örnek Müşteri",
                    BoatName = "Mavi Rüya",
                    TourDate = DateTime.Now.AddDays(-1).ToString("dd.MM.yyyy"),
                    ActionUrl = "https://www.e-iskele.com/hesabim/yorumlar"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("ReviewRequest", model, cancellationToken);
                subject = "e-iskele: Turunuz Nasıldı?";
            }
            else
            {
                return Result.Failure("ScenarioNotReady", "Bu senaryo için test şablonu henüz hazır değil.");
            }

            var assembly = typeof(SmtpEmailSettingsService).Assembly;
            using var logoStream = assembly.GetManifestResourceStream("EIskele.Infrastructure.Resources.e-iskele_logo.png");
            byte[]? logoBytes = null;
            if (logoStream != null)
            {
                using var ms = new System.IO.MemoryStream();
                await logoStream.CopyToAsync(ms, cancellationToken);
                logoBytes = ms.ToArray();
            }

            await _emailSender.SendAsync(email, subject, htmlBody, dto, logoBytes, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure("SmtpSendError", ex.Message);
        }
    }
}
"@
Set-Content -Path "$infraSettingsDir\SmtpEmailSettingsService.cs" -Value $emailImpl -Encoding UTF8

# 6. NotificationSettingsService
$notificationImpl = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Common.Settings;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Settings;

public class NotificationSettingsService : INotificationSettingsService
{
    private readonly EIskeleDbContext _dbContext;

    public NotificationSettingsService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<NotificationSettingsDto>> GetNotificationSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
        
        bool GetValueBool(string key, bool def) => bool.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;

        var dto = new NotificationSettingsDto
        {
            EmailNotificationsEnabled = GetValueBool("Notification.EmailEnabled", true),
            SmsNotificationsEnabled = GetValueBool("Notification.SmsEnabled", false),
            PushNotificationsEnabled = GetValueBool("Notification.PushEnabled", false),
            WhatsAppNotificationsEnabled = GetValueBool("Notification.WhatsAppEnabled", false),
            InAppNotificationsEnabled = GetValueBool("Notification.InAppEnabled", false),
            Scenarios = new List<NotificationScenarioDto>
            {
                new() { Key = "UserRegistered", Name = "Kullanıcı kayıt oldu", Channels = new() { "email" }, Status = "ready" },
                new() { Key = "CaptainApplicationReceived", Name = "Kaptan başvurusu alındı", Channels = new() { "email" }, Status = "ready" },
                new() { Key = "CaptainApplicationApproved", Name = "Kaptan başvurusu onaylandı", Channels = new() { "email", "sms" }, Status = "ready" },
                new() { Key = "MissingDocumentRequested", Name = "Eksik belge istendi", Channels = new() { "email" }, Status = "ready" },
                new() { Key = "BoatApproved", Name = "Tekne onaylandı", Channels = new() { "email" }, Status = "ready" },
                new() { Key = "ReservationCreated", Name = "Rezervasyon oluşturuldu", Channels = new() { "email", "sms" }, Status = "ready" },
                new() { Key = "ReservationApproved", Name = "Rezervasyon onaylandı", Channels = new() { "email", "sms" }, Status = "ready" },
                new() { Key = "ReservationRejected", Name = "Rezervasyon reddedildi", Channels = new() { "email" }, Status = "ready" },
                new() { Key = "PaymentSuccessful", Name = "Ödeme başarılı", Channels = new() { "email" }, Status = "preparation" },
                new() { Key = "TourReminder24h", Name = "Turdan 24 saat önce hatırlatma", Channels = new() { "email", "push" }, Status = "preparation" }
            }
        };

        return Result<NotificationSettingsDto>.Success(dto);
    }

    public async Task<Result> UpdateNotificationSettingsAsync(NotificationSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.ToListAsync(cancellationToken);

        void UpdateValue(string key, string value, string valueType)
        {
            var setting = settings.FirstOrDefault(s => s.Key == key);
            if (setting == null)
            {
                _dbContext.SystemSettings.Add(new Domain.Entities.SystemSetting
                {
                    Key = key,
                    Value = value,
                    ValueType = Enum.Parse<SystemSettingValueType>(valueType, true),
                    Group = "Notification",
                    Description = "",
                    IsEditable = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId
                });
            }
            else if (setting.Value != value)
            {
                setting.Value = value;
                setting.UpdatedAt = DateTime.UtcNow;
                setting.UpdatedBy = currentUserId;
            }
        }

        UpdateValue("Notification.EmailEnabled", dto.EmailNotificationsEnabled.ToString(), "Boolean");
        UpdateValue("Notification.SmsEnabled", dto.SmsNotificationsEnabled.ToString(), "Boolean");
        UpdateValue("Notification.PushEnabled", dto.PushNotificationsEnabled.ToString(), "Boolean");
        UpdateValue("Notification.WhatsAppEnabled", dto.WhatsAppNotificationsEnabled.ToString(), "Boolean");
        UpdateValue("Notification.InAppEnabled", dto.InAppNotificationsEnabled.ToString(), "Boolean");

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
"@
Set-Content -Path "$infraSettingsDir\NotificationSettingsService.cs" -Value $notificationImpl -Encoding UTF8

# 7. SecuritySettingsService
$securityImpl = @"
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Common.Settings;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Settings;

public class SecuritySettingsService : ISecuritySettingsService
{
    private readonly EIskeleDbContext _dbContext;

    public SecuritySettingsService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<SecuritySettingsDto>> GetSecuritySettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
        
        int GetValueInt(string key, int def) => int.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;
        bool GetValueBool(string key, bool def) => bool.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;

        var dto = new SecuritySettingsDto
        {
            AdminSessionMinutes = GetValueInt("Security.AdminSessionMinutes", 60),
            RefreshTokenDays = GetValueInt("Security.RefreshTokenDays", 7),
            MaxFailedLoginAttempts = GetValueInt("Security.MaxFailedLoginAttempts", 5),
            AccountLockoutMinutes = GetValueInt("Security.AccountLockoutMinutes", 15),
            PasswordMinimumLength = GetValueInt("Security.PasswordMinimumLength", 8),
            RequirePasswordComplexity = GetValueBool("Security.RequirePasswordComplexity", true),
            AuditLogEnabled = GetValueBool("Security.AuditLogEnabled", true)
        };

        return Result<SecuritySettingsDto>.Success(dto);
    }

    public async Task<Result> UpdateSecuritySettingsAsync(SecuritySettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.ToListAsync(cancellationToken);

        void UpdateValue(string key, string value, string valueType)
        {
            var setting = settings.FirstOrDefault(s => s.Key == key);
            if (setting == null)
            {
                _dbContext.SystemSettings.Add(new Domain.Entities.SystemSetting
                {
                    Key = key,
                    Value = value,
                    ValueType = valueType == "Number" ? SystemSettingValueType.Int : Enum.Parse<SystemSettingValueType>(valueType, true),
                    Group = "Security",
                    Description = "",
                    IsEditable = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId
                });
            }
            else if (setting.Value != value)
            {
                setting.Value = value;
                setting.UpdatedAt = DateTime.UtcNow;
                setting.UpdatedBy = currentUserId;
            }
        }

        UpdateValue("Security.AdminSessionMinutes", dto.AdminSessionMinutes.ToString(), "Number");
        UpdateValue("Security.RefreshTokenDays", dto.RefreshTokenDays.ToString(), "Number");
        UpdateValue("Security.MaxFailedLoginAttempts", dto.MaxFailedLoginAttempts.ToString(), "Number");
        UpdateValue("Security.AccountLockoutMinutes", dto.AccountLockoutMinutes.ToString(), "Number");
        UpdateValue("Security.PasswordMinimumLength", dto.PasswordMinimumLength.ToString(), "Number");
        UpdateValue("Security.RequirePasswordComplexity", dto.RequirePasswordComplexity.ToString(), "Boolean");
        UpdateValue("Security.AuditLogEnabled", dto.AuditLogEnabled.ToString(), "Boolean");

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
"@
Set-Content -Path "$infraSettingsDir\SecuritySettingsService.cs" -Value $securityImpl -Encoding UTF8

# 8. PaymentSettingsService
$paymentImpl = @"
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Common.Settings;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Settings;

public class PaymentSettingsService : IPaymentSettingsService
{
    private readonly EIskeleDbContext _dbContext;

    public PaymentSettingsService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PaymentSettingsDto>> GetPaymentSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
        
        bool GetValueBool(string key, bool def) => bool.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;
        string GetValueString(string key, string def) => settings.FirstOrDefault(s => s.Key == key)?.Value ?? def;

        var dto = new PaymentSettingsDto
        {
            PaymentEnabled = GetValueBool("Payment.PaymentEnabled", false),
            PaymentProvider = GetValueString("Payment.PaymentProvider", "none"),
            PaymentTestMode = GetValueBool("Payment.PaymentTestMode", true),
            Require3DSecure = GetValueBool("Payment.Require3DSecure", true),
            DepositPaymentEnabled = GetValueBool("Payment.DepositPaymentEnabled", true),
            FullPaymentEnabled = GetValueBool("Payment.FullPaymentEnabled", false),
            RefundManagementEnabled = GetValueBool("Payment.RefundManagementEnabled", false)
        };

        return Result<PaymentSettingsDto>.Success(dto);
    }

    public async Task<Result> UpdatePaymentSettingsAsync(PaymentSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.ToListAsync(cancellationToken);

        void UpdateValue(string key, string value, string valueType)
        {
            var setting = settings.FirstOrDefault(s => s.Key == key);
            if (setting == null)
            {
                _dbContext.SystemSettings.Add(new Domain.Entities.SystemSetting 
                { 
                    Key = key, 
                    Value = value, 
                    ValueType = valueType == "Number" ? SystemSettingValueType.Int : Enum.Parse<SystemSettingValueType>(valueType, true), 
                    Group = "Payment", Description = "", IsEditable = true, CreatedAt = DateTime.UtcNow, CreatedBy = currentUserId 
                });
            }
            else if (setting.Value != value)
            {
                setting.Value = value; setting.UpdatedAt = DateTime.UtcNow; setting.UpdatedBy = currentUserId;
            }
        }

        UpdateValue("Payment.PaymentEnabled", dto.PaymentEnabled.ToString(), "Boolean");
        UpdateValue("Payment.PaymentProvider", dto.PaymentProvider, "String");
        UpdateValue("Payment.PaymentTestMode", dto.PaymentTestMode.ToString(), "Boolean");
        UpdateValue("Payment.Require3DSecure", dto.Require3DSecure.ToString(), "Boolean");
        UpdateValue("Payment.DepositPaymentEnabled", dto.DepositPaymentEnabled.ToString(), "Boolean");
        UpdateValue("Payment.FullPaymentEnabled", dto.FullPaymentEnabled.ToString(), "Boolean");
        UpdateValue("Payment.RefundManagementEnabled", dto.RefundManagementEnabled.ToString(), "Boolean");

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
"@
Set-Content -Path "$infraSettingsDir\PaymentSettingsService.cs" -Value $paymentImpl -Encoding UTF8

# 9. SmsSettingsService
$smsImpl = @"
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Common.Settings;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Settings;

public class SmsSettingsService : ISmsSettingsService
{
    private readonly EIskeleDbContext _dbContext;

    public SmsSettingsService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<SmsSettingsDto>> GetSmsSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
        
        bool GetValueBool(string key, bool def) => bool.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;
        string GetValueString(string key, string def) => settings.FirstOrDefault(s => s.Key == key)?.Value ?? def;

        var dto = new SmsSettingsDto
        {
            SmsEnabled = GetValueBool("Sms.SmsEnabled", false),
            SmsProvider = GetValueString("Sms.SmsProvider", "none"),
            SmsSenderTitle = GetValueString("Sms.SmsSenderTitle", "EISKELE")
        };

        return Result<SmsSettingsDto>.Success(dto);
    }

    public async Task<Result> UpdateSmsSettingsAsync(SmsSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.ToListAsync(cancellationToken);

        void UpdateValue(string key, string value, string valueType)
        {
            var setting = settings.FirstOrDefault(s => s.Key == key);
            if (setting == null)
            {
                _dbContext.SystemSettings.Add(new Domain.Entities.SystemSetting 
                { 
                    Key = key, Value = value, ValueType = valueType == "Number" ? SystemSettingValueType.Int : Enum.Parse<SystemSettingValueType>(valueType, true), Group = "Sms", Description = "", IsEditable = true, CreatedAt = DateTime.UtcNow, CreatedBy = currentUserId 
                });
            }
            else if (setting.Value != value)
            {
                setting.Value = value; setting.UpdatedAt = DateTime.UtcNow; setting.UpdatedBy = currentUserId;
            }
        }

        UpdateValue("Sms.SmsEnabled", dto.SmsEnabled.ToString(), "Boolean");
        UpdateValue("Sms.SmsProvider", dto.SmsProvider, "String");
        UpdateValue("Sms.SmsSenderTitle", dto.SmsSenderTitle, "String");

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
"@
Set-Content -Path "$infraSettingsDir\SmsSettingsService.cs" -Value $smsImpl -Encoding UTF8

# 10. MaintenanceModeSettingsService
$maintenanceImpl = @"
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Common.Settings;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Settings;

public class MaintenanceModeSettingsService : IMaintenanceModeSettingsService
{
    private readonly EIskeleDbContext _dbContext;

    public MaintenanceModeSettingsService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<MaintenanceModeSettingsDto>> GetMaintenanceModeSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
        
        bool GetValueBool(string key, bool def) => bool.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;
        string GetValueString(string key, string def) => settings.FirstOrDefault(s => s.Key == key)?.Value ?? def;

        var dto = new MaintenanceModeSettingsDto
        {
            MaintenanceModeEnabled = GetValueBool("Maintenance.MaintenanceModeEnabled", false),
            MaintenanceMessage = GetValueString("Maintenance.MaintenanceMessage", "e-iskele kısa süreli bakım nedeniyle geçici olarak hizmet veremiyor."),
            MaintenanceAffectsCustomerWeb = GetValueBool("Maintenance.MaintenanceAffectsCustomerWeb", true),
            MaintenanceAffectsCaptainHub = GetValueBool("Maintenance.MaintenanceAffectsCaptainHub", true),
            MaintenanceAffectsAdminPanel = GetValueBool("Maintenance.MaintenanceAffectsAdminPanel", false),
            MaintenanceAffectsPublicApi = GetValueBool("Maintenance.MaintenanceAffectsPublicApi", true)
        };

        return Result<MaintenanceModeSettingsDto>.Success(dto);
    }

    public async Task<Result> UpdateMaintenanceModeSettingsAsync(MaintenanceModeSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.ToListAsync(cancellationToken);

        void UpdateValue(string key, string value, string valueType)
        {
            var setting = settings.FirstOrDefault(s => s.Key == key);
            if (setting == null)
            {
                _dbContext.SystemSettings.Add(new Domain.Entities.SystemSetting 
                { 
                    Key = key, Value = value, ValueType = valueType == "Number" ? SystemSettingValueType.Int : Enum.Parse<SystemSettingValueType>(valueType, true), Group = "Maintenance", Description = "", IsEditable = true, CreatedAt = DateTime.UtcNow, CreatedBy = currentUserId 
                });
            }
            else if (setting.Value != value)
            {
                setting.Value = value; setting.UpdatedAt = DateTime.UtcNow; setting.UpdatedBy = currentUserId;
            }
        }

        UpdateValue("Maintenance.MaintenanceModeEnabled", dto.MaintenanceModeEnabled.ToString(), "Boolean");
        UpdateValue("Maintenance.MaintenanceMessage", dto.MaintenanceMessage, "String");
        UpdateValue("Maintenance.MaintenanceAffectsCustomerWeb", dto.MaintenanceAffectsCustomerWeb.ToString(), "Boolean");
        UpdateValue("Maintenance.MaintenanceAffectsCaptainHub", dto.MaintenanceAffectsCaptainHub.ToString(), "Boolean");
        UpdateValue("Maintenance.MaintenanceAffectsAdminPanel", dto.MaintenanceAffectsAdminPanel.ToString(), "Boolean");
        UpdateValue("Maintenance.MaintenanceAffectsPublicApi", dto.MaintenanceAffectsPublicApi.ToString(), "Boolean");

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
"@
Set-Content -Path "$infraSettingsDir\MaintenanceModeSettingsService.cs" -Value $maintenanceImpl -Encoding UTF8

# 11. SettingsAuditLogService
$auditLogImpl = @"
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Common.Settings;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Settings;

public class SettingsAuditLogService : ISettingsAuditLogService
{
    private readonly EIskeleDbContext _dbContext;

    public SettingsAuditLogService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<SettingsAuditLogDto>>> GetSettingsAuditLogsAsync(CancellationToken cancellationToken = default)
    {
        var logs = await _dbContext.AuditLogs
            .AsNoTracking()
            .Where(a => a.EntityType == "SystemSetting")
            .OrderByDescending(a => a.CreatedAt)
            .Take(50)
            .Select(a => new SettingsAuditLogDto
            {
                Id = a.Id.ToString(),
                Action = a.Action,
                SettingGroup = "System Settings",
                OldValue = a.OldValue ?? "",
                NewValue = a.NewValue ?? "",
                ActorName = a.ActorUserId.ToString() ?? "",
                ActorIp = a.IpAddress ?? "",
                CreatedAt = a.CreatedAt,
                Status = "success"
            })
            .ToListAsync(cancellationToken);

        return Result<List<SettingsAuditLogDto>>.Success(logs);
    }
}
"@
Set-Content -Path "$infraSettingsDir\SettingsAuditLogService.cs" -Value $auditLogImpl -Encoding UTF8
