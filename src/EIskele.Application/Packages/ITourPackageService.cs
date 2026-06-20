using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Packages;

public interface ITourPackageService
{
    // Admin Methods
    Task<Result<PackageDetailDto>> GetAdminPackageDetailAsync(Guid id);
    Task<Result> ApprovePackageAsync(Guid id);
    Task<Result> RejectPackageAsync(Guid id, string reason);
    Task<Result> RequestRevisionAsync(Guid id, string[] fields, string note);
    Task<Result> DeactivatePackageAsync(Guid id, string reason);
    Task<Result> SuspendPackageAsync(Guid id, string reason);
    Task<Result> ReactivatePackageAsync(Guid id);
    Task<Result> AddAdminNoteAsync(Guid id, string noteType, string noteText);

    // Captain Methods
    Task<Result<List<CaptainPackageListItemDto>>> GetMyPackagesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<CaptainPackageDetailDto>> GetMyPackageDetailAsync(Guid packageId, Guid userId, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateMyPackageAsync(Guid userId, CreateCaptainPackageRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateMyPackageAsync(Guid packageId, Guid userId, UpdateCaptainPackageRequest request, CancellationToken cancellationToken = default);
    Task<Result> ActivateMyPackageAsync(Guid packageId, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> DeactivateMyPackageAsync(Guid packageId, Guid userId, CancellationToken cancellationToken = default);
    Task<Result<Guid>> DuplicateMyPackageAsync(Guid packageId, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> DeleteMyPackageAsync(Guid packageId, Guid userId, CancellationToken cancellationToken = default);
}
