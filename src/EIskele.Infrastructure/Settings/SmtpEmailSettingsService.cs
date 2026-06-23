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
                    
                return Result.Failure("M365AuthError", "Microsoft 365 token alÄ±namadÄ±.");
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

            await _emailSender.SendAsync(email, "e-iskele SMTP Test E-postasÄ±", htmlBody, dto, logoBytes, cancellationToken);

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
                    UserName = "Test KullanÄ±cÄ±sÄ±"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("UserRegistered", model, cancellationToken);
                subject = "e-iskele: HoÅŸ Geldiniz";
            }
            else if (scenarioKey == "password_reset")
            {
                var model = new EIskele.Infrastructure.Emails.Models.PasswordResetEmailModel
                {
                    UserName = "Test KullanÄ±cÄ±sÄ±",
                    ActionUrl = "https://www.e-iskele.com/sifre-sifirla?token=test"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("PasswordReset", model, cancellationToken);
                subject = "e-iskele: Åifre SÄ±fÄ±rlama Talebi";
            }
            else if (scenarioKey == "captain_application_received")
            {
                var model = new EIskele.Infrastructure.Emails.Models.CaptainApplicationReceivedEmailModel
                {
                    CaptainName = "Ã–rnek Kaptan",
                    ApplicationDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                    SupportUrl = "https://kaptanhub.e-iskele.com/destek"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("CaptainApplicationReceived", model, cancellationToken);
                subject = "e-iskele: Kaptan BaÅŸvurunuz AlÄ±ndÄ±";
            }
            else if (scenarioKey == "captain_application_approved")
            {
                var model = new EIskele.Infrastructure.Emails.Models.CaptainApplicationResultEmailModel
                {
                    CaptainName = "Ã–rnek Kaptan",
                    SupportUrl = "https://kaptanhub.e-iskele.com/"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("CaptainApplicationApproved", model, cancellationToken);
                subject = "e-iskele: Kaptan BaÅŸvurunuz OnaylandÄ±";
            }
            else if (scenarioKey == "captain_application_rejected")
            {
                var model = new EIskele.Infrastructure.Emails.Models.CaptainApplicationResultEmailModel
                {
                    CaptainName = "Ã–rnek Kaptan",
                    Reason = "Belgeleriniz doÄŸrulanamadÄ±. LÃ¼tfen geÃ§erli bir ticari ruhsat yÃ¼kleyiniz.",
                    SupportUrl = "https://kaptanhub.e-iskele.com/destek"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("CaptainApplicationRejected", model, cancellationToken);
                subject = "e-iskele: Kaptan BaÅŸvurunuz Reddedildi";
            }
            else if (scenarioKey == "captain_application_missing_document")
            {
                var model = new EIskele.Infrastructure.Emails.Models.CaptainApplicationResultEmailModel
                {
                    CaptainName = "Ã–rnek Kaptan",
                    Reason = "Kimlik fotokopiniz okunaksÄ±zdÄ±r. LÃ¼tfen daha net bir fotoÄŸraf yÃ¼kleyiniz.",
                    SupportUrl = "https://kaptanhub.e-iskele.com/belgeler"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("CaptainApplicationMissingDocument", model, cancellationToken);
                subject = "e-iskele: Eksik Belge Talebi";
            }
            else if (scenarioKey == "boat_published")
            {
                var model = new EIskele.Infrastructure.Emails.Models.BoatStatusEmailModel
                {
                    CaptainName = "Ã–rnek Kaptan",
                    BoatName = "Mavi RÃ¼ya",
                    ActionUrl = "https://kaptanhub.e-iskele.com/tekneler"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("BoatPublished", model, cancellationToken);
                subject = "e-iskele: Tekneniz YayÄ±na AlÄ±ndÄ±";
            }
            else if (scenarioKey == "boat_rejected")
            {
                var model = new EIskele.Infrastructure.Emails.Models.BoatStatusEmailModel
                {
                    CaptainName = "Ã–rnek Kaptan",
                    BoatName = "Mavi RÃ¼ya",
                    Reason = "Tekne gÃ¶rselleri kalite standartlarÄ±mÄ±za uymamaktadÄ±r. LÃ¼tfen daha yÃ¼ksek Ã§Ã¶zÃ¼nÃ¼rlÃ¼klÃ¼ gÃ¶rseller ekleyiniz.",
                    ActionUrl = "https://kaptanhub.e-iskele.com/tekneler"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("BoatRejected", model, cancellationToken);
                subject = "e-iskele: Tekne YayÄ±na AlÄ±mÄ± Reddedildi";
            }
            else if (scenarioKey == "reservation_created_customer")
            {
                var model = new EIskele.Infrastructure.Emails.Models.ReservationEmailModel
                {
                    RecipientName = "Ã–rnek MÃ¼ÅŸteri",
                    BoatName = "Mavi RÃ¼ya",
                    PackageName = "Tam GÃ¼n BalÄ±k Turu",
                    ReservationDate = DateTime.Now.AddDays(7).ToString("dd.MM.yyyy HH:mm"),
                    GuestCount = 4,
                    TotalPrice = 5000.00m,
                    StatusMessage = "Rezervasyon talebiniz kaptana iletildi. Kaptan onayladÄ±ÄŸÄ±nda Ã¶deme adÄ±mÄ±na geÃ§ebileceksiniz.",
                    ActionUrl = "https://www.e-iskele.com/hesabim/rezervasyonlar"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("ReservationCreatedCustomer", model, cancellationToken);
                subject = "e-iskele: Rezervasyon Talebiniz AlÄ±ndÄ±";
            }
            else if (scenarioKey == "reservation_created_captain")
            {
                var model = new EIskele.Infrastructure.Emails.Models.ReservationEmailModel
                {
                    RecipientName = "Ã–rnek Kaptan",
                    BoatName = "Mavi RÃ¼ya",
                    PackageName = "Tam GÃ¼n BalÄ±k Turu",
                    ReservationDate = DateTime.Now.AddDays(7).ToString("dd.MM.yyyy HH:mm"),
                    GuestCount = 4,
                    TotalPrice = 5000.00m,
                    StatusMessage = "LÃ¼tfen 24 saat iÃ§erisinde talebi onaylayÄ±n veya reddedin.",
                    ActionUrl = "https://kaptanhub.e-iskele.com/rezervasyonlar"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("ReservationCreatedCaptain", model, cancellationToken);
                subject = "e-iskele: Yeni Rezervasyon Talebi Var";
            }
            else if (scenarioKey == "reservation_approved")
            {
                var model = new EIskele.Infrastructure.Emails.Models.ReservationEmailModel
                {
                    RecipientName = "Ã–rnek MÃ¼ÅŸteri",
                    BoatName = "Mavi RÃ¼ya",
                    ReservationDate = DateTime.Now.AddDays(7).ToString("dd.MM.yyyy HH:mm"),
                    TotalPrice = 5000.00m,
                    StatusMessage = "Talebiniz kaptan tarafÄ±ndan onaylandÄ±! LÃ¼tfen Ã¶demenizi tamamlayarak rezervasyonunuzu kesinleÅŸtirin.",
                    ActionUrl = "https://www.e-iskele.com/hesabim/rezervasyonlar"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("ReservationApproved", model, cancellationToken);
                subject = "e-iskele: Rezervasyonunuz OnaylandÄ±";
            }
            else if (scenarioKey == "reservation_rejected")
            {
                var model = new EIskele.Infrastructure.Emails.Models.ReservationEmailModel
                {
                    RecipientName = "Ã–rnek MÃ¼ÅŸteri",
                    BoatName = "Mavi RÃ¼ya",
                    ReservationDate = DateTime.Now.AddDays(7).ToString("dd.MM.yyyy HH:mm"),
                    StatusMessage = "KaptanÄ±mÄ±z belirttiÄŸiniz tarihlerde teknenin mÃ¼sait olmadÄ±ÄŸÄ±nÄ± bildirmiÅŸtir.",
                    ActionUrl = "https://www.e-iskele.com/turlar"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("ReservationRejected", model, cancellationToken);
                subject = "e-iskele: Rezervasyon Talebiniz OnaylanamadÄ±";
            }
            else if (scenarioKey == "reservation_cancelled")
            {
                var model = new EIskele.Infrastructure.Emails.Models.ReservationEmailModel
                {
                    RecipientName = "Ã–rnek KullanÄ±cÄ±",
                    BoatName = "Mavi RÃ¼ya",
                    ReservationDate = DateTime.Now.AddDays(7).ToString("dd.MM.yyyy HH:mm"),
                    StatusMessage = "Ä°ptal talebiniz iÅŸleme alÄ±nmÄ±ÅŸ olup iptal politikasÄ±na gÃ¶re sÃ¼reÃ§ baÅŸlatÄ±lmÄ±ÅŸtÄ±r.",
                    ActionUrl = "https://www.e-iskele.com/hesabim/rezervasyonlar"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("ReservationCancelled", model, cancellationToken);
                subject = "e-iskele: Rezervasyonunuz Ä°ptal Edildi";
            }
            else if (scenarioKey == "reservation_reminder")
            {
                var model = new EIskele.Infrastructure.Emails.Models.ReservationEmailModel
                {
                    RecipientName = "Ã–rnek MÃ¼ÅŸteri",
                    BoatName = "Mavi RÃ¼ya",
                    PackageName = "Tam GÃ¼n BalÄ±k Turu",
                    ReservationDate = DateTime.Now.AddDays(1).ToString("dd.MM.yyyy HH:mm"),
                    StatusMessage = "Harika bir deniz deneyimi yaÅŸamanÄ±z iÃ§in kaptanÄ±mÄ±z sizi bekliyor olacak.",
                    ActionUrl = "https://www.e-iskele.com/hesabim/rezervasyonlar"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("ReservationReminder", model, cancellationToken);
                subject = "e-iskele: Turunuz YaklaÅŸÄ±yor!";
            }
            else if (scenarioKey == "payment_success")
            {
                var model = new EIskele.Infrastructure.Emails.Models.PaymentEmailModel
                {
                    CustomerName = "Ã–rnek MÃ¼ÅŸteri",
                    ReservationId = "RES-98765432",
                    BoatName = "Mavi RÃ¼ya",
                    Amount = 5000.00m,
                    ActionUrl = "https://www.e-iskele.com/hesabim/rezervasyonlar"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("PaymentSuccess", model, cancellationToken);
                subject = "e-iskele: Ã–demeniz BaÅŸarÄ±yla AlÄ±ndÄ±";
            }
            else if (scenarioKey == "payment_failed")
            {
                var model = new EIskele.Infrastructure.Emails.Models.PaymentEmailModel
                {
                    CustomerName = "Ã–rnek MÃ¼ÅŸteri",
                    ReservationId = "RES-98765432",
                    BoatName = "Mavi RÃ¼ya",
                    Amount = 5000.00m,
                    ActionUrl = "https://www.e-iskele.com/hesabim/rezervasyonlar"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("PaymentFailed", model, cancellationToken);
                subject = "e-iskele: Ã–deme BaÅŸarÄ±sÄ±z Oldu";
            }
            else if (scenarioKey == "review_request")
            {
                var model = new EIskele.Infrastructure.Emails.Models.ReviewRequestEmailModel
                {
                    CustomerName = "Ã–rnek MÃ¼ÅŸteri",
                    BoatName = "Mavi RÃ¼ya",
                    TourDate = DateTime.Now.AddDays(-1).ToString("dd.MM.yyyy"),
                    ActionUrl = "https://www.e-iskele.com/hesabim/yorumlar"
                };
                htmlBody = await _emailTemplateRenderer.RenderAsync("ReviewRequest", model, cancellationToken);
                subject = "e-iskele: Turunuz NasÄ±ldÄ±?";
            }
            else
            {
                return Result.Failure("ScenarioNotReady", "Bu senaryo iÃ§in test ÅŸablonu henÃ¼z hazÄ±r deÄŸil.");
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
