using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Availability;

public interface IAvailabilityService
{
    Task<Result<IEnumerable<AvailabilitySlotDto>>> CheckAvailabilityAsync(CheckAvailabilityRequest request, CancellationToken cancellationToken = default);
    Task<Result> BlockDatesAsync(BlockDatesRequest request, CancellationToken cancellationToken = default);
}
