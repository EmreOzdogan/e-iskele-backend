using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Common.Settings;

public interface ISettingsAuditLogService
{
    Task<Result<List<SettingsAuditLogDto>>> GetSettingsAuditLogsAsync(CancellationToken cancellationToken = default);
}
