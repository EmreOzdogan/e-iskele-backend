using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Audit;
using EIskele.Domain.Entities;
using EIskele.Persistence;
using Microsoft.AspNetCore.Http;

namespace EIskele.Infrastructure.Audit;

public class AuditLogService : IAuditLogService
{
    private readonly EIskeleDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogService(EIskeleDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(AuditLogRequest request, CancellationToken cancellationToken = default)
    {
        var traceId = request.TraceId;
        if (string.IsNullOrEmpty(traceId) && _httpContextAccessor.HttpContext != null)
        {
            traceId = _httpContextAccessor.HttpContext.TraceIdentifier;
        }

        var ipAddress = request.IpAddress;
        if (string.IsNullOrEmpty(ipAddress) && _httpContextAccessor.HttpContext != null)
        {
            ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        }

        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            ActorUserId = request.ActorUserId,
            ActorRole = request.ActorRole ?? string.Empty,
            Action = request.Action,
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            OldValue = request.OldValue ?? string.Empty,
            NewValue = request.NewValue ?? string.Empty,
            Description = request.Description ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            IpAddress = ipAddress ?? string.Empty,
            TraceId = traceId ?? string.Empty
        };

        _dbContext.AuditLogs.Add(auditLog);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
