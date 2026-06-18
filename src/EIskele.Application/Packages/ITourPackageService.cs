using System;
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
}
