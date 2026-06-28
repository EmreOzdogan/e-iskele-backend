using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Companies;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Companies;

public class CompanyService : ICompanyService
{
    private readonly EIskeleDbContext _dbContext;

    public CompanyService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<CompanyDto>>> GetCompaniesAsync(int page = 1, int pageSize = 20, string? search = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Companies.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.CompanyName.Contains(search) || x.TaxNumber.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new CompanyDto
            {
                Id = x.Id,
                CompanyName = x.CompanyName,
                TaxNumber = x.TaxNumber,
                TaxOffice = x.TaxOffice,
                Address = x.Address,
                AuthorizedPersonName = x.AuthorizedPersonName,
                Iban = x.Iban,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<CompanyDto>>.Success(new PagedResult<CompanyDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }

    public async Task<Result<CompanyDto>> GetCompanyByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var company = await _dbContext.Companies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (company == null) return Result<CompanyDto>.Failure("NOT_FOUND", "Firma bulunamadı.");

        return Result<CompanyDto>.Success(new CompanyDto
        {
            Id = company.Id,
            CompanyName = company.CompanyName,
            TaxNumber = company.TaxNumber,
            TaxOffice = company.TaxOffice,
            Address = company.Address,
            AuthorizedPersonName = company.AuthorizedPersonName,
            Iban = company.Iban,
            CreatedAt = company.CreatedAt
        });
    }

    public async Task<Result> UpdateCompanyAsync(Guid id, UpdateCompanyDto dto, CancellationToken cancellationToken = default)
    {
        var company = await _dbContext.Companies.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (company == null) return Result.Failure("NOT_FOUND", "Firma bulunamadı.");

        company.CompanyName = dto.CompanyName;
        company.TaxNumber = dto.TaxNumber;
        company.TaxOffice = dto.TaxOffice;
        company.Address = dto.Address;
        company.AuthorizedPersonName = dto.AuthorizedPersonName;
        company.Iban = dto.Iban;
        company.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
