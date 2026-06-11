using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Settings;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Identity.Client;
using MimeKit;
namespace EIskele.Infrastructure.Settings;

public class SettingsService : ISettingsService
{
    private readonly EIskeleDbContext _dbContext;
    private readonly EIskele.Infrastructure.Emails.Services.IEmailTemplateRenderer _emailTemplateRenderer;
    private readonly EIskele.Infrastructure.Emails.Services.IEmailSender _emailSender;

    public SettingsService(
        EIskeleDbContext dbContext,
        EIskele.Infrastructure.Emails.Services.IEmailTemplateRenderer emailTemplateRenderer,
        EIskele.Infrastructure.Emails.Services.IEmailSender emailSender)
    {
        _dbContext = dbContext;
        _emailTemplateRenderer = emailTemplateRenderer;
        _emailSender = emailSender;
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
        {
            return defaultValue;
        }

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

    public async Task<EIskele.Application.Common.Results.Result<SystemSettingsDto>> GetGeneralSettingsAsync(CancellationToken cancellationToken = default)
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

        return EIskele.Application.Common.Results.Result<SystemSettingsDto>.Success(dto);
    }

    public async Task<EIskele.Application.Common.Results.Result> UpdateGeneralSettingsAsync(SystemSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.ToListAsync(cancellationToken);

        void UpdateValue(string key, string? value)
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
                        ValueType = "String",
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

        return EIskele.Application.Common.Results.Result.Success();
    }

    public async Task<EIskele.Application.Common.Results.Result<ReservationRulesSettingsDto>> GetReservationRulesSettingsAsync(CancellationToken cancellationToken = default)
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

        return EIskele.Application.Common.Results.Result<ReservationRulesSettingsDto>.Success(dto);
    }

    public async Task<EIskele.Application.Common.Results.Result> UpdateReservationRulesSettingsAsync(ReservationRulesSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
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
                    ValueType = valueType,
                    Group = "ReservationRules",
                    Description = "",
                    IsEditable = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId
                });
            }
            else
            {
                if (setting.Value != value)
                {
                    setting.Value = value;
                    setting.UpdatedAt = DateTime.UtcNow;
                    setting.UpdatedBy = currentUserId;
                }
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

        return EIskele.Application.Common.Results.Result.Success();
    }

    public async Task<EIskele.Application.Common.Results.Result<CommissionFinanceSettingsDto>> GetCommissionFinanceSettingsAsync(CancellationToken cancellationToken = default)
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

        return EIskele.Application.Common.Results.Result<CommissionFinanceSettingsDto>.Success(dto);
    }

    public async Task<EIskele.Application.Common.Results.Result> UpdateCommissionFinanceSettingsAsync(CommissionFinanceSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
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
                    ValueType = valueType,
                    Group = "CommissionFinance",
                    Description = "",
                    IsEditable = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId
                });
            }
            else
            {
                if (setting.Value != value)
                {
                    setting.Value = value;
                    setting.UpdatedAt = DateTime.UtcNow;
                    setting.UpdatedBy = currentUserId;
                }
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

        return EIskele.Application.Common.Results.Result.Success();
    }
    public async Task<EIskele.Application.Common.Results.Result<SmtpEmailSettingsDto>> GetSmtpEmailSettingsAsync(CancellationToken cancellationToken = default)
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

        return EIskele.Application.Common.Results.Result<SmtpEmailSettingsDto>.Success(dto);
    }

    public async Task<EIskele.Application.Common.Results.Result> UpdateSmtpEmailSettingsAsync(SmtpEmailSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
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
                    ValueType = valueType,
                    Group = "SmtpEmail",
                    Description = "",
                    IsEditable = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId
                });
            }
            else
            {
                if (setting.Value != value)
                {
                    setting.Value = value;
                    setting.UpdatedAt = DateTime.UtcNow;
                    setting.UpdatedBy = currentUserId;
                }
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

        return EIskele.Application.Common.Results.Result.Success();
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

    public async Task<EIskele.Application.Common.Results.Result> TestSmtpConnectionAsync(SmtpEmailSettingsDto dto, CancellationToken cancellationToken = default)
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
                    return EIskele.Application.Common.Results.Result.Success();
                    
                return EIskele.Application.Common.Results.Result.Failure("M365AuthError", "Microsoft 365 token alınamadı.");
            }

            using var client = new SmtpClient();
            var secureOption = GetSecureSocketOption(dto.SmtpSecurityType);

            await client.ConnectAsync(dto.SmtpHost, dto.SmtpPort ?? 587, secureOption, cancellationToken);
            
            await AuthenticateSmtpClientAsync(client, dto, cancellationToken);

            await client.DisconnectAsync(true, cancellationToken);
            return EIskele.Application.Common.Results.Result.Success();
        }
        catch (Exception ex)
        {
            return EIskele.Application.Common.Results.Result.Failure("SmtpConnectionError", ex.Message);
        }
    }

    public async Task<EIskele.Application.Common.Results.Result> SendTestEmailAsync(string email, SmtpEmailSettingsDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = new EIskele.Infrastructure.Emails.Models.TestEmailModel
            {
                BaseUrl = "https://www.e-iskele.com",
                AdminPanelUrl = "https://admin.e-iskele.com"
            };

            var htmlBody = await _emailTemplateRenderer.RenderAsync("TestEmail", model, cancellationToken);

            var assembly = typeof(SettingsService).Assembly;
            using var logoStream = assembly.GetManifestResourceStream("EIskele.Infrastructure.Resources.e-iskele_logo.png");
            byte[]? logoBytes = null;
            if (logoStream != null)
            {
                using var ms = new System.IO.MemoryStream();
                await logoStream.CopyToAsync(ms, cancellationToken);
                logoBytes = ms.ToArray();
            }

            await _emailSender.SendAsync(email, "e-iskele SMTP Test E-postası", htmlBody, dto, logoBytes, cancellationToken);

            return EIskele.Application.Common.Results.Result.Success();
        }
        catch (Exception ex)
        {
            return EIskele.Application.Common.Results.Result.Failure("SmtpSendError", ex.Message);
        }
    }

    public async Task<EIskele.Application.Common.Results.Result<NotificationSettingsDto>> GetNotificationSettingsAsync(CancellationToken cancellationToken = default)
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

        return EIskele.Application.Common.Results.Result<NotificationSettingsDto>.Success(dto);
    }

    public async Task<EIskele.Application.Common.Results.Result> UpdateNotificationSettingsAsync(NotificationSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
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
                    ValueType = valueType,
                    Group = "Notification",
                    Description = "",
                    IsEditable = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId
                });
            }
            else
            {
                if (setting.Value != value)
                {
                    setting.Value = value;
                    setting.UpdatedAt = DateTime.UtcNow;
                    setting.UpdatedBy = currentUserId;
                }
            }
        }

        UpdateValue("Notification.EmailEnabled", dto.EmailNotificationsEnabled.ToString(), "Boolean");
        UpdateValue("Notification.SmsEnabled", dto.SmsNotificationsEnabled.ToString(), "Boolean");
        UpdateValue("Notification.PushEnabled", dto.PushNotificationsEnabled.ToString(), "Boolean");
        UpdateValue("Notification.WhatsAppEnabled", dto.WhatsAppNotificationsEnabled.ToString(), "Boolean");
        UpdateValue("Notification.InAppEnabled", dto.InAppNotificationsEnabled.ToString(), "Boolean");

        await _dbContext.SaveChangesAsync(cancellationToken);

        return EIskele.Application.Common.Results.Result.Success();
    }

    public async Task<EIskele.Application.Common.Results.Result<SecuritySettingsDto>> GetSecuritySettingsAsync(CancellationToken cancellationToken = default)
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

        return EIskele.Application.Common.Results.Result<SecuritySettingsDto>.Success(dto);
    }

    public async Task<EIskele.Application.Common.Results.Result> UpdateSecuritySettingsAsync(SecuritySettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
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
                    ValueType = valueType,
                    Group = "Security",
                    Description = "",
                    IsEditable = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId
                });
            }
            else
            {
                if (setting.Value != value)
                {
                    setting.Value = value;
                    setting.UpdatedAt = DateTime.UtcNow;
                    setting.UpdatedBy = currentUserId;
                }
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

        return EIskele.Application.Common.Results.Result.Success();
    }

    public async Task<EIskele.Application.Common.Results.Result<PaymentSettingsDto>> GetPaymentSettingsAsync(CancellationToken cancellationToken = default)
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

        return EIskele.Application.Common.Results.Result<PaymentSettingsDto>.Success(dto);
    }

    public async Task<EIskele.Application.Common.Results.Result> UpdatePaymentSettingsAsync(PaymentSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.ToListAsync(cancellationToken);

        void UpdateValue(string key, string value, string valueType)
        {
            var setting = settings.FirstOrDefault(s => s.Key == key);
            if (setting == null)
            {
                _dbContext.SystemSettings.Add(new Domain.Entities.SystemSetting { Key = key, Value = value, ValueType = valueType, Group = "Payment", Description = "", IsEditable = true, CreatedAt = DateTime.UtcNow, CreatedBy = currentUserId });
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
        return EIskele.Application.Common.Results.Result.Success();
    }

    public async Task<EIskele.Application.Common.Results.Result<SmsSettingsDto>> GetSmsSettingsAsync(CancellationToken cancellationToken = default)
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

        return EIskele.Application.Common.Results.Result<SmsSettingsDto>.Success(dto);
    }

    public async Task<EIskele.Application.Common.Results.Result> UpdateSmsSettingsAsync(SmsSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.ToListAsync(cancellationToken);

        void UpdateValue(string key, string value, string valueType)
        {
            var setting = settings.FirstOrDefault(s => s.Key == key);
            if (setting == null)
            {
                _dbContext.SystemSettings.Add(new Domain.Entities.SystemSetting { Key = key, Value = value, ValueType = valueType, Group = "Sms", Description = "", IsEditable = true, CreatedAt = DateTime.UtcNow, CreatedBy = currentUserId });
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
        return EIskele.Application.Common.Results.Result.Success();
    }

    public async Task<EIskele.Application.Common.Results.Result<MaintenanceModeSettingsDto>> GetMaintenanceModeSettingsAsync(CancellationToken cancellationToken = default)
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

        return EIskele.Application.Common.Results.Result<MaintenanceModeSettingsDto>.Success(dto);
    }

    public async Task<EIskele.Application.Common.Results.Result> UpdateMaintenanceModeSettingsAsync(MaintenanceModeSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.ToListAsync(cancellationToken);

        void UpdateValue(string key, string value, string valueType)
        {
            var setting = settings.FirstOrDefault(s => s.Key == key);
            if (setting == null)
            {
                _dbContext.SystemSettings.Add(new Domain.Entities.SystemSetting { Key = key, Value = value, ValueType = valueType, Group = "Maintenance", Description = "", IsEditable = true, CreatedAt = DateTime.UtcNow, CreatedBy = currentUserId });
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
        return EIskele.Application.Common.Results.Result.Success();
    }

    public async Task<EIskele.Application.Common.Results.Result<System.Collections.Generic.List<SettingsAuditLogDto>>> GetSettingsAuditLogsAsync(CancellationToken cancellationToken = default)
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
                SettingGroup = "System Settings", // Bu değer log verilerinden daha da detaylandırılabilir
                OldValue = a.OldValue ?? "",
                NewValue = a.NewValue ?? "",
                ActorName = a.ActorUserId.ToString() ?? "",
                ActorIp = a.IpAddress ?? "",
                CreatedAt = a.CreatedAt,
                Status = "success"
            })
            .ToListAsync(cancellationToken);

        return EIskele.Application.Common.Results.Result<System.Collections.Generic.List<SettingsAuditLogDto>>.Success(logs);
    }
}
