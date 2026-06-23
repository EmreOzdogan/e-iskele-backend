using EIskele.Application.Common.Locations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers;

[ApiController]
[Route("api/locations")]
[AllowAnonymous]
public class LocationsController : BaseController
{
    private readonly ILocationService _locationService;
    private readonly IHarborService _harborService;

    public LocationsController(ILocationService locationService, IHarborService harborService)
    {
        _locationService = locationService;
        _harborService = harborService;
    }

    [HttpGet]
    public async Task<IActionResult> GetActiveLocations(CancellationToken cancellationToken)
    {
        var result = await _locationService.GetActiveLocationsAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("harbors")]
    public async Task<IActionResult> GetActiveHarbors(CancellationToken cancellationToken)
    {
        var result = await _harborService.GetActiveHarborsAsync(cancellationToken);
        return HandleResult(result);
    }
}
