using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Common.Settings;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Settings;

public class SettingsAuditLogService : ISettingsAuditLogService
{
    private readonly EIskeleDbContext _dbContext;

    public SettingsAuditLogService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<SettingsAuditLogDto>>> GetSettingsAuditLogsAsync(CancellationToken cancellationToken = default)
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
                SettingGroup = "System Settings",
                OldValue = a.OldValue ?? "",
                NewValue = a.NewValue ?? "",
                ActorName = a.ActorUserId.ToString() ?? "",
                ActorIp = a.IpAddress ?? "",
                CreatedAt = a.CreatedAt,
                Status = "success"
            })
            .ToListAsync(cancellationToken);

        return Result<List<SettingsAuditLogDto>>.Success(logs);
    }
}
