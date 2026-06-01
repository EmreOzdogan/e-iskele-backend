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
}
