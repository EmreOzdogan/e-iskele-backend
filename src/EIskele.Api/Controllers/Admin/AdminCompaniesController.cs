using EIskele.Application.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/companies")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminCompaniesController : BaseController
{
    private readonly ICompanyService _companyService;

    public AdminCompaniesController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCompanies(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _companyService.GetCompaniesAsync(page, pageSize, search, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCompanyById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _companyService.GetCompanyByIdAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCompany(
        Guid id,
        [FromBody] UpdateCompanyDto dto,
        CancellationToken cancellationToken = default)
    {
        var result = await _companyService.UpdateCompanyAsync(id, dto, cancellationToken);
        return HandleResult(result);
    }
}
