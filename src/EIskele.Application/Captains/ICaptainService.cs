using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Captains;

public interface ICaptainService
{
    Task<Result<CaptainApplicationResponse>> ApplyAsync(CaptainApplicationRequest request, CancellationToken cancellationToken = default);
    Task<Result> ApproveApplicationAsync(Guid applicationId, CancellationToken cancellationToken = default);
    Task<Result> RejectApplicationAsync(Guid applicationId, string reason, CancellationToken cancellationToken = default);

    // Admin methods
    Task<Result<AdminCaptainsSummaryDto>> GetAdminCaptainsSummaryAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<AdminCaptainListItemDto>>> GetAdminCaptainsAsync(GetAdminCaptainsQuery query, CancellationToken cancellationToken = default);
    Task<Result<AdminCaptainDetailDto>> GetAdminCaptainDetailAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> SuspendCaptainAsync(Guid id, string reason, CancellationToken cancellationToken = default);
    Task<Result> ReactivateCaptainAsync(Guid id, CancellationToken cancellationToken = default);
}
