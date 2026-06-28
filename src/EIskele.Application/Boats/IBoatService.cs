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

    // Captain methods
    Task<Result<List<CaptainBoatListItemDto>>> GetMyBoatsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<CaptainBoatDetailDto>> GetMyBoatDetailAsync(Guid boatId, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> UpdateMyBoatAsync(Guid boatId, Guid userId, UpdateCaptainBoatRequest request, CancellationToken cancellationToken = default);
    Task<Result<BoatResponse>> CreateMyBoatAsync(Guid userId, CreateCaptainBoatRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeactivateMyBoatAsync(Guid boatId, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> DeleteMyBoatAsync(Guid boatId, Guid userId, CancellationToken cancellationToken = default);


    Task<Result<PagedResult<AdminBoatListItemDto>>> GetAdminBoatsAsync(
        string? search, string? boatStatus, string? documentStatus, string? publishStatus,
        string? captainStatus, Guid? locationId, string? boatType,
        int? minCapacity, int? maxCapacity, int page, int pageSize,
        string? sortBy, string? sortDirection, CancellationToken cancellationToken = default);

    Task<Result<AdminBoatsSummaryDto>> GetAdminBoatsSummaryAsync(CancellationToken cancellationToken = default);

    Task<Result<BoatDetailDto>> GetAdminBoatDetailAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<BoatImageDto>>> GetAdminBoatImagesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<BoatDocumentDto>>> GetAdminBoatDocumentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<BoatFeatureDto>>> GetAdminBoatFeaturesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<BoatPackageSummaryDto>>> GetAdminBoatPackagesAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<Result> RejectBoatAsync(Guid id, string reason, CancellationToken cancellationToken = default);
    Task<Result> RequestBoatRevisionAsync(Guid id, List<string> fields, string note, CancellationToken cancellationToken = default);
    Task<Result> DeactivateBoatAsync(Guid id, string reason, CancellationToken cancellationToken = default);
    Task<Result> SuspendBoatAsync(Guid id, string reason, CancellationToken cancellationToken = default);
    Task<Result> ReactivateBoatAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result> ApproveBoatImageAsync(Guid imageId, CancellationToken cancellationToken = default);
    Task<Result> RejectBoatImageAsync(Guid imageId, string reason, CancellationToken cancellationToken = default);
    Task<Result> ApproveBoatDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task<Result> RejectBoatDocumentAsync(Guid documentId, string reason, CancellationToken cancellationToken = default);
}
