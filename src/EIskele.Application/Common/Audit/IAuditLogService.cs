using System.Threading;
using System.Threading.Tasks;

namespace EIskele.Application.Common.Audit;

public interface IAuditLogService
{
    Task LogAsync(AuditLogRequest request, CancellationToken cancellationToken = default);
}
