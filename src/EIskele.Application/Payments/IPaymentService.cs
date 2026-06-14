using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Payments;

public interface IPaymentService
{
    Task<Result<AdminPaymentSummaryMetricsDto>> GetAdminPaymentsSummaryAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<AdminPaymentListItemDto>>> GetAdminPaymentsAsync(GetAdminPaymentsQuery query, CancellationToken cancellationToken = default);
    Task<Result<AdminPaymentDetailDto>> GetAdminPaymentDetailAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> ProcessRefundRequestAsync(Guid id, string action, string reason, CancellationToken cancellationToken = default);
    Task<Result> UpdatePayoutStatusAsync(Guid id, string status, string? reason, CancellationToken cancellationToken = default);
}
