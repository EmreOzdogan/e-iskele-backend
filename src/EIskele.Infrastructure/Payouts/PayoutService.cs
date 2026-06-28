using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Payouts;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Payouts;

public class PayoutService : IPayoutService
{
    private readonly EIskeleDbContext _dbContext;

    public PayoutService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<PayoutDto>>> GetCaptainPayoutsAsync(Guid captainUserId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var captain = await _dbContext.Captains.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == captainUserId, cancellationToken);
        if (captain == null) return Result<PagedResult<PayoutDto>>.Failure("NOT_FOUND", "Kaptan profili bulunamadı.");

        var query = _dbContext.Payouts.AsNoTracking().Where(x => x.CaptainId == captain.Id);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new PayoutDto
            {
                Id = x.Id,
                PayoutNo = x.PayoutNo,
                Amount = x.Amount,
                IbanMasked = x.IbanMasked,
                Status = x.Status.ToString(),
                ScheduledDate = x.ScheduledDate,
                PaidDate = x.PaidDate,
                CaptainId = x.CaptainId,
                CaptainName = x.Captain.User.FirstName + " " + x.Captain.User.LastName,
                RelatedReservationCount = x.RelatedReservationCount,
                Description = x.Description
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<PayoutDto>>.Success(new PagedResult<PayoutDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }

    public async Task<Result<PagedResult<PayoutDto>>> GetAdminPayoutsAsync(int page = 1, int pageSize = 20, string? search = null, string? status = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Payouts.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.PayoutNo.Contains(search) || x.Captain.User.FirstName.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<PayoutStatus>(status, out var statusEnum))
        {
            query = query.Where(x => x.Status == statusEnum);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new PayoutDto
            {
                Id = x.Id,
                PayoutNo = x.PayoutNo,
                Amount = x.Amount,
                IbanMasked = x.IbanMasked,
                Status = x.Status.ToString(),
                ScheduledDate = x.ScheduledDate,
                PaidDate = x.PaidDate,
                CaptainId = x.CaptainId,
                CaptainName = x.Captain.User.FirstName + " " + x.Captain.User.LastName,
                RelatedReservationCount = x.RelatedReservationCount,
                Description = x.Description
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<PayoutDto>>.Success(new PagedResult<PayoutDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }

    public async Task<Result> UpdatePayoutStatusAsync(Guid id, UpdatePayoutStatusDto dto, Guid adminId, CancellationToken cancellationToken = default)
    {
        var payout = await _dbContext.Payouts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (payout == null) return Result.Failure("NOT_FOUND", "Hak ediş kaydı bulunamadı.");

        if (!Enum.TryParse<PayoutStatus>(dto.Status, out var statusEnum))
            return Result.Failure("VALIDATION_ERROR", "Geçersiz durum.");

        payout.Status = statusEnum;
        if (statusEnum == PayoutStatus.Paid)
        {
            payout.PaidDate = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
