using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Common.Audit;

public interface IAuditLogService
{
    Task LogAsync(AuditLogRequest request, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<AuditLogDto>>> GetAuditLogsAsync(int page = 1, int pageSize = 20, string? search = null, CancellationToken cancellationToken = default);
}
