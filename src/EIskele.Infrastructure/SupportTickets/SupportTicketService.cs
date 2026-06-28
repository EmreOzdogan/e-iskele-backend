using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.SupportTickets;
using EIskele.Domain.Entities;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.SupportTickets;

public class SupportTicketService : ISupportTicketService
{
    private readonly EIskeleDbContext _dbContext;

    public SupportTicketService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<SupportTicketDto>>> GetAdminTicketsAsync(int page = 1, int pageSize = 20, string? search = null, string? status = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.SupportTickets.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.TicketNo.Contains(search) || x.Subject.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<SupportTicketStatus>(status, out var statusEnum))
        {
            query = query.Where(x => x.Status == statusEnum);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new SupportTicketDto
            {
                Id = x.Id,
                TicketNo = x.TicketNo,
                Subject = x.Subject,
                Category = x.Category.ToString(),
                Priority = x.Priority.ToString(),
                Status = x.Status.ToString(),
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<SupportTicketDto>>.Success(new PagedResult<SupportTicketDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }

    public async Task<Result<PagedResult<SupportTicketDto>>> GetMyTicketsAsync(Guid userId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.SupportTickets.AsNoTracking().Where(x => x.UserId == userId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new SupportTicketDto
            {
                Id = x.Id,
                TicketNo = x.TicketNo,
                Subject = x.Subject,
                Category = x.Category.ToString(),
                Priority = x.Priority.ToString(),
                Status = x.Status.ToString(),
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<SupportTicketDto>>.Success(new PagedResult<SupportTicketDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }

    public async Task<Result<SupportTicketDto>> CreateTicketAsync(CreateSupportTicketDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<SupportTicketCategory>(dto.Category, out var categoryEnum))
            categoryEnum = SupportTicketCategory.Other;

        if (!Enum.TryParse<SupportTicketPriority>(dto.Priority, out var priorityEnum))
            priorityEnum = SupportTicketPriority.Low;

        var ticket = new SupportTicket
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TicketNo = $"T-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}",
            Subject = dto.Subject,
            Category = categoryEnum,
            Priority = priorityEnum,
            Status = SupportTicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.SupportTickets.Add(ticket);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<SupportTicketDto>.Success(new SupportTicketDto
        {
            Id = ticket.Id,
            TicketNo = ticket.TicketNo,
            Subject = ticket.Subject,
            Category = ticket.Category.ToString(),
            Priority = ticket.Priority.ToString(),
            Status = ticket.Status.ToString(),
            CreatedAt = ticket.CreatedAt
        });
    }

    public async Task<Result> UpdateTicketStatusAsync(Guid id, UpdateSupportTicketStatusDto dto, Guid adminId, CancellationToken cancellationToken = default)
    {
        var ticket = await _dbContext.SupportTickets.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (ticket == null) return Result.Failure("NOT_FOUND", "Destek talebi bulunamadı.");

        if (!Enum.TryParse<SupportTicketStatus>(dto.Status, out var statusEnum))
            return Result.Failure("VALIDATION_ERROR", "Geçersiz durum.");

        ticket.Status = statusEnum;
        ticket.UpdatedAt = DateTime.UtcNow;
        ticket.UpdatedBy = adminId;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
