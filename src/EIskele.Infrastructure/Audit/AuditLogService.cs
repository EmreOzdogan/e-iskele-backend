using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Audit;
using EIskele.Application.Common.Results;
using EIskele.Domain.Entities;
using EIskele.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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

    public async Task<Result<PagedResult<AuditLogDto>>> GetAuditLogsAsync(int page = 1, int pageSize = 20, string? search = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.AuditLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => 
                x.Action.Contains(search) || 
                x.EntityType.Contains(search) || 
                x.Description.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AuditLogDto
            {
                Id = x.Id,
                ActorUserId = x.ActorUserId,
                ActorRole = x.ActorRole,
                Action = x.Action,
                EntityType = x.EntityType,
                EntityId = x.EntityId,
                OldValue = x.OldValue,
                NewValue = x.NewValue,
                Description = x.Description,
                CreatedAt = x.CreatedAt,
                IpAddress = x.IpAddress,
                TraceId = x.TraceId
            })
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<AuditLogDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return Result<PagedResult<AuditLogDto>>.Success(pagedResult);
    }
}
