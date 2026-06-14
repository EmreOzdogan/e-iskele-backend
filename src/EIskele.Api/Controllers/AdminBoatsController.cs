using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Boats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers;

[Route("api/admin/boats")]
public class AdminBoatsController : BaseController
{
    private readonly IBoatService _boatService;

    public AdminBoatsController(IBoatService boatService)
    {
        _boatService = boatService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBoats(
        [FromQuery] string? search,
        [FromQuery] string? boatStatus,
        [FromQuery] string? documentStatus,
        [FromQuery] string? publishStatus,
        [FromQuery] string? captainStatus,
        [FromQuery] Guid? locationId,
        [FromQuery] string? boatType,
        [FromQuery] int? minCapacity,
        [FromQuery] int? maxCapacity,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortDirection = "desc",
        CancellationToken cancellationToken = default)
    {
        var result = await _boatService.GetAdminBoatsAsync(
            search, boatStatus, documentStatus, publishStatus, captainStatus, locationId, boatType,
            minCapacity, maxCapacity, page, pageSize, sortBy, sortDirection, cancellationToken);
        
        return HandleResult(result);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetBoatsSummary(CancellationToken cancellationToken)
    {
        var result = await _boatService.GetAdminBoatsSummaryAsync(cancellationToken);
        return HandleResult(result);
    }
}
