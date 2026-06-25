using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Reservations;
using EIskele.Persistence;

namespace EIskele.Infrastructure.Services;

public class ReservationQueryService : IReservationQueryService
{
    private readonly EIskeleDbContext _dbContext;

    public ReservationQueryService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Result<IEnumerable<ReservationResponse>>> GetReservationsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result<IEnumerable<ReservationResponse>>.Failure("NOT_IMPLEMENTED", "Metod henüz kodlanmadı."));
    }
}
