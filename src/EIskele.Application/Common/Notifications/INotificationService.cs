using System.Threading;
using System.Threading.Tasks;

namespace EIskele.Application.Common.Notifications;

public interface INotificationService
{
    Task<NotificationResult> SendAsync(NotificationRequest request, CancellationToken cancellationToken = default);
}
