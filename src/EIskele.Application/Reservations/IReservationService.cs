using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Reservations;

public interface IReservationService
{
    Task<Result<ReservationResponse>> CreateReservationAsync(CreateReservationRequest request, CancellationToken cancellationToken = default);
    Task<Result> ApproveReservationAsync(Guid reservationId, CancellationToken cancellationToken = default);
    Task<Result> CancelReservationAsync(Guid reservationId, string reason, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ReservationResponse>>> GetReservationsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    
    // Admin methods
    Task<Result<AdminReservationSummaryMetricsDto>> GetAdminReservationsSummaryAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<AdminReservationListItemDto>>> GetAdminReservationsAsync(GetAdminReservationsQuery query, CancellationToken cancellationToken = default);
    Task<Result<AdminReservationDetailDto>> GetAdminReservationDetailAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> AdminCancelReservationAsync(Guid id, string reason, CancellationToken cancellationToken = default);
    Task<Result> AdminPostponeReservationAsync(Guid id, DateTime newDate, CancellationToken cancellationToken = default);
}
