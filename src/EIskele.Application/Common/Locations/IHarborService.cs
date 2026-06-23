using EIskele.Application.Common.Results;
using EIskele.Domain.Enums;

namespace EIskele.Application.Common.Locations;

public interface IHarborService
{
    Task<Result<List<LocationHarborListItemDto>>> GetLocationHarborsAsync(Guid locationId, CancellationToken cancellationToken);
    Task<Result<Guid>> CreateHarborAsync(CreateHarborDto dto, Guid currentUserId, CancellationToken cancellationToken);
    Task<Result> UpdateHarborAsync(UpdateHarborDto dto, Guid currentUserId, CancellationToken cancellationToken);
    Task<Result> DeleteHarborAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken);
    Task<Result<List<ActiveHarborDto>>> GetActiveHarborsAsync(CancellationToken cancellationToken);
}
