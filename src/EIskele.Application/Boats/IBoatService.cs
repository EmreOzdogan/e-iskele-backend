using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Boats;

public interface IBoatService
{
    Task<Result<BoatResponse>> CreateBoatAsync(CreateBoatRequest request, CancellationToken cancellationToken = default);
    Task<Result> SubmitForReviewAsync(Guid boatId, CancellationToken cancellationToken = default);
    Task<Result> ApproveBoatAsync(Guid boatId, CancellationToken cancellationToken = default);
    Task<Result<TourPackageResponse>> AddTourPackageAsync(CreateTourPackageRequest request, CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AdminBoatListItemDto>>> GetAdminBoatsAsync(
        string? search, string? boatStatus, string? documentStatus, string? publishStatus,
        string? captainStatus, Guid? locationId, string? boatType,
        int? minCapacity, int? maxCapacity, int page, int pageSize,
        string? sortBy, string? sortDirection, CancellationToken cancellationToken = default);

    Task<Result<AdminBoatsSummaryDto>> GetAdminBoatsSummaryAsync(CancellationToken cancellationToken = default);
}
