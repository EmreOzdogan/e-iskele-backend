using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Availability;

public interface ICaptainCalendarService
{
    Task<Result<CaptainCalendarMetricsDto>> GetMetricsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<CaptainCalendarSlotDto>>> GetSlotsAsync(Guid userId, CaptainCalendarFilterDto filters, CancellationToken cancellationToken = default);
    Task<Result> AddAvailabilityBlockAsync(Guid userId, AddAvailabilityBlockRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAvailabilityBlockAsync(Guid userId, Guid slotId, CancellationToken cancellationToken = default);
}
