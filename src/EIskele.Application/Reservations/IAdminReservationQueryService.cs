using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Reservations;

public interface IAdminReservationQueryService
{
    Task<Result<AdminReservationSummaryMetricsDto>> GetAdminReservationsSummaryAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<AdminReservationListItemDto>>> GetAdminReservationsAsync(GetAdminReservationsQuery query, CancellationToken cancellationToken = default);
    Task<Result<AdminReservationDetailDto>> GetAdminReservationDetailAsync(Guid id, CancellationToken cancellationToken = default);
}
