using EIskele.Application.Common.Results;
using EIskele.Domain.Enums;

namespace EIskele.Application.Common.Locations;

public interface ILocationService
{
    Task<Result<PagedResult<AdminLocationListItemDto>>> GetAdminLocationsAsync(
        string? search, LocationType? type, LocationStatus? status, bool? isPopular,
        LocationSeoStatus? seoStatus, LocationCoordinateStatus? coordinateStatus,
        LocationRegion? region, int? minBoatCount, int? maxBoatCount,
        int page, int pageSize, string? sortBy, string? sortDirection, CancellationToken cancellationToken);

    Task<Result<AdminLocationsSummaryDto>> GetAdminLocationsSummaryAsync(CancellationToken cancellationToken);

    Task<Result<AdminLocationDetailDto>> GetLocationDetailAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<Guid>> CreateLocationAsync(CreateLocationDto dto, Guid currentUserId, CancellationToken cancellationToken);

    Task<Result> UpdateLocationAsync(UpdateLocationDto dto, Guid currentUserId, CancellationToken cancellationToken);

    Task<Result> DeleteLocationAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken);

    Task<Result> ActivateLocationAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken);
    Task<Result> DeactivateLocationAsync(Guid id, string? reason, Guid currentUserId, CancellationToken cancellationToken);
    Task<Result> MarkLocationPopularAsync(Guid id, string? note, int? order, Guid currentUserId, CancellationToken cancellationToken);
    Task<Result> UnmarkLocationPopularAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken);

    Task<Result<List<LocationHarborListItemDto>>> GetLocationHarborsAsync(Guid locationId, CancellationToken cancellationToken);

    Task<Result<Guid>> CreateHarborAsync(CreateHarborDto dto, Guid currentUserId, CancellationToken cancellationToken);

    Task<Result> UpdateHarborAsync(UpdateHarborDto dto, Guid currentUserId, CancellationToken cancellationToken);

    Task<Result> DeleteHarborAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken);
}
