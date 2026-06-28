using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Companies;

public interface ICompanyService
{
    Task<Result<PagedResult<CompanyDto>>> GetCompaniesAsync(int page = 1, int pageSize = 20, string? search = null, CancellationToken cancellationToken = default);
    Task<Result<CompanyDto>> GetCompanyByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> UpdateCompanyAsync(Guid id, UpdateCompanyDto dto, CancellationToken cancellationToken = default);
}
