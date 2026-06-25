using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Reservations;

public interface IReservationCommandService
{
    Task<Result<ReservationResponse>> CreateReservationAsync(CreateReservationRequest request, CancellationToken cancellationToken = default);
    Task<Result> ApproveReservationAsync(Guid reservationId, CancellationToken cancellationToken = default);
    Task<Result> CancelReservationAsync(Guid reservationId, string reason, CancellationToken cancellationToken = default);
}
