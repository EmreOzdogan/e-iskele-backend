using System;
using System.Threading;
using System.Threading.Tasks;

namespace EIskele.Infrastructure.Notifications;

public class EmailNotificationSender : INotificationSender
{
    public string Channel => "Email";

    public Task<bool> SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        // TODO: SmtpClient veya MailKit entegrasyonu buraya gelecek.
        // MVP aşamasında sadece console'a yazdırıyor ve başarılı varsayıyoruz.
        Console.WriteLine($"[EMAIL] To: {to} | Subject: {subject}");
        Console.WriteLine($"[EMAIL] Body: {body}");
        return Task.FromResult(true);
    }
}
