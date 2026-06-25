using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Reservations;

public interface IReservationQueryService
{
    Task<Result<IEnumerable<ReservationResponse>>> GetReservationsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
