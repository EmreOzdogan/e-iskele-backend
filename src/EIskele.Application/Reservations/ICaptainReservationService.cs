using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Reservations;

public interface ICaptainReservationService
{
    Task<Result<IEnumerable<CaptainReservationListItemDto>>> GetReservationsAsync(Guid userId, string status = "all", CancellationToken cancellationToken = default);
    Task<Result<CaptainReservationDetailDto>> GetReservationDetailAsync(Guid userId, Guid reservationId, CancellationToken cancellationToken = default);
    Task<Result> ApproveReservationAsync(Guid userId, Guid reservationId, string? note, CancellationToken cancellationToken = default);
    Task<Result> RejectReservationAsync(Guid userId, Guid reservationId, string reason, CancellationToken cancellationToken = default);
    Task<Result> CancelReservationAsync(Guid userId, Guid reservationId, string reason, CancellationToken cancellationToken = default);
    Task<Result> UpdateCaptainNoteAsync(Guid userId, Guid reservationId, string note, CancellationToken cancellationToken = default);
}
