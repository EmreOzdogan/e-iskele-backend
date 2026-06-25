using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Reservations;

public interface IAdminReservationCommandService
{
    Task<Result> AdminCancelReservationAsync(Guid id, string reason, CancellationToken cancellationToken = default);
    Task<Result> AdminPostponeReservationAsync(Guid id, DateTime newDate, CancellationToken cancellationToken = default);
}
