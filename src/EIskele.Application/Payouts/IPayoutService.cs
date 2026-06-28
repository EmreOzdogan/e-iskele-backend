using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Payouts;

public interface IPayoutService
{
    Task<Result<PagedResult<PayoutDto>>> GetCaptainPayoutsAsync(Guid captainUserId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<PayoutDto>>> GetAdminPayoutsAsync(int page = 1, int pageSize = 20, string? search = null, string? status = null, CancellationToken cancellationToken = default);
    Task<Result> UpdatePayoutStatusAsync(Guid id, UpdatePayoutStatusDto dto, Guid adminId, CancellationToken cancellationToken = default);
}
