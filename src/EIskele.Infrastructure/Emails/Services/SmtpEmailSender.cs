using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Identity.Client;
using MimeKit;

namespace EIskele.Infrastructure.Emails.Services;

public class SmtpEmailSender : IEmailSender
{
    public async Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        SmtpEmailSettingsDto settings,
        byte[]? inlineLogoBytes = null,
        CancellationToken cancellationToken = default)
    {
        if (settings.SmtpProvider == "m365" && !string.IsNullOrEmpty(settings.M365ClientId) && !string.IsNullOrEmpty(settings.M365TenantId) && !string.IsNullOrEmpty(settings.M365ClientSecret))
        {
            await SendViaGraphApiAsync(to, subject, htmlBody, settings, inlineLogoBytes, cancellationToken);
            return;
        }

        await SendViaMailKitAsync(to, subject, htmlBody, settings, inlineLogoBytes, cancellationToken);
    }

    private async Task SendViaGraphApiAsync(string to, string subject, string htmlBody, SmtpEmailSettingsDto dto, byte[]? inlineLogoBytes, CancellationToken cancellationToken)
    {
        var cca = ConfidentialClientApplicationBuilder
            .Create(dto.M365ClientId)
            .WithClientSecret(dto.M365ClientSecret)
            .WithTenantId(dto.M365TenantId)
            .Build();

        var authResult = await cca.AcquireTokenForClient(new[] { "https://graph.microsoft.com/.default" })
            .ExecuteAsync(cancellationToken);

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.AccessToken);

        object? replyTo = string.IsNullOrEmpty(dto.ReplyToEmail) ? null : new[]
        {
            new { emailAddress = new { address = dto.ReplyToEmail } }
        };

        var messageObj = new Dictionary<string, object>
        {
            { "subject", subject },
            { "body", new { contentType = "HTML", content = htmlBody } },
            { "toRecipients", new[] { new { emailAddress = new { address = to } } } }
        };

        if (replyTo != null)
        {
            messageObj.Add("replyTo", replyTo);
        }

        if (inlineLogoBytes != null)
        {
            var attachments = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "@odata.type", "#microsoft.graph.fileAttachment" },
                    { "name", "e-iskele_logo.png" },
                    { "contentType", "image/png" },
                    { "contentBytes", Convert.ToBase64String(inlineLogoBytes) },
                    { "isInline", true },
                    { "contentId", "e-iskele-logo" }
                }
            };
            messageObj.Add("attachments", attachments);
        }

        var payload = new
        {
            message = messageObj,
            saveToSentItems = false
        };

        var jsonOptions = new System.Text.Json.JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };
        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload, jsonOptions), System.Text.Encoding.UTF8, "application/json");
        
        var fromUser = !string.IsNullOrEmpty(dto.SmtpUsername) ? dto.SmtpUsername : dto.SenderEmail;
        var url = $"https://graph.microsoft.com/v1.0/users/{fromUser}/sendMail";
        
        var response = await httpClient.PostAsync(url, content, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Graph API Hatası: {response.StatusCode} - {errorBody}");
        }
    }

    private async Task SendViaMailKitAsync(string to, string subject, string htmlBody, SmtpEmailSettingsDto dto, byte[]? inlineLogoBytes, CancellationToken cancellationToken)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(dto.SenderName ?? "e-iskele", dto.SenderEmail));
        message.To.Add(new MailboxAddress("", to));

        if (!string.IsNullOrEmpty(dto.ReplyToEmail))
        {
            message.ReplyTo.Add(new MailboxAddress("", dto.ReplyToEmail));
        }

        message.Subject = subject;

        var builder = new BodyBuilder();
        builder.HtmlBody = htmlBody;

        if (inlineLogoBytes != null)
        {
            var image = builder.LinkedResources.Add("e-iskele_logo.png", inlineLogoBytes, new ContentType("image", "png"));
            image.ContentId = "e-iskele-logo";
        }

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        var secureOption = GetSecureSocketOption(dto.SmtpSecurityType);
        
        var port = dto.SmtpPort ?? 587;
        await client.ConnectAsync(dto.SmtpHost, port, secureOption, cancellationToken);

        if (!string.IsNullOrEmpty(dto.SmtpUsername) && !string.IsNullOrEmpty(dto.SmtpPassword))
        {
            await client.AuthenticateAsync(dto.SmtpUsername, dto.SmtpPassword, cancellationToken);
        }

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }

    private SecureSocketOptions GetSecureSocketOption(string? securityType)
    {
        return securityType?.ToLower() switch
        {
            "ssl" => SecureSocketOptions.SslOnConnect,
            "tls" => SecureSocketOptions.StartTls,
            "none" => SecureSocketOptions.None,
            _ => SecureSocketOptions.Auto
        };
    }
}
