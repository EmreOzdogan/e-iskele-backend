using System.Threading;
using System.Threading.Tasks;

namespace EIskele.Infrastructure.Notifications;

public interface INotificationSender
{
    string Channel { get; }
    Task<bool> SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}
