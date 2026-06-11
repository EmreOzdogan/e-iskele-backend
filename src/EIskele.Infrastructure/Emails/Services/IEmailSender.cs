using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Settings;

namespace EIskele.Infrastructure.Emails.Services;

public interface IEmailSender
{
    Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        SmtpEmailSettingsDto settings,
        byte[]? inlineLogoBytes = null,
        CancellationToken cancellationToken = default);
}
