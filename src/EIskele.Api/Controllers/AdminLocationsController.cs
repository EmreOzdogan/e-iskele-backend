using EIskele.Application.Common.Locations;
using EIskele.Application.Common.Results;
using EIskele.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EIskele.Api.Controllers;

[ApiController]
[Route("api/admin/locations")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminLocationsController : BaseController
{
    private readonly ILocationService _locationService;
    private readonly IHarborService _harborService;

    public AdminLocationsController(ILocationService locationService, IHarborService harborService)
    {
        _locationService = locationService;
        _harborService = harborService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLocations(
        [FromQuery] string? search,
        [FromQuery] LocationType? type,
        [FromQuery] LocationStatus? status,
        [FromQuery] bool? isPopular,
        [FromQuery] LocationSeoStatus? seoStatus,
        [FromQuery] LocationCoordinateStatus? coordinateStatus,
        [FromQuery] LocationRegion? region,
        [FromQuery] int? minBoatCount,
        [FromQuery] int? maxBoatCount,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _locationService.GetAdminLocationsAsync(search, type, status, isPopular, seoStatus, coordinateStatus, region, minBoatCount, maxBoatCount, page, pageSize, sortBy, sortDirection, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var result = await _locationService.GetAdminLocationsSummaryAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLocation(Guid id, CancellationToken cancellationToken)
    {
        var result = await _locationService.GetLocationDetailAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLocation([FromBody] CreateLocationDto dto, CancellationToken cancellationToken)
    {
        var userId = this.UserId;
        var result = await _locationService.CreateLocationAsync(dto, userId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLocation(Guid id, [FromBody] UpdateLocationDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id) return HandleResult(Result.Failure("VALIDATION_ERROR", "Id eşleşmiyor."));

        var userId = this.UserId;
        var result = await _locationService.UpdateLocationAsync(dto, userId, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLocation(Guid id, CancellationToken cancellationToken)
    {
        var userId = this.UserId;
        var result = await _locationService.DeleteLocationAsync(id, userId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> ActivateLocation(Guid id, CancellationToken cancellationToken)
    {
        var userId = this.UserId;
        var result = await _locationService.ActivateLocationAsync(id, userId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateLocation(Guid id, [FromBody] DeactivateLocationDto dto, CancellationToken cancellationToken)
    {
        var userId = this.UserId;
        var result = await _locationService.DeactivateLocationAsync(id, dto.Reason, userId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id}/mark-popular")]
    public async Task<IActionResult> MarkLocationPopular(Guid id, [FromBody] MarkLocationPopularDto dto, CancellationToken cancellationToken)
    {
        var userId = this.UserId;
        var result = await _locationService.MarkLocationPopularAsync(id, dto.Note, dto.Order, userId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id}/unmark-popular")]
    public async Task<IActionResult> UnmarkLocationPopular(Guid id, CancellationToken cancellationToken)
    {
        var userId = this.UserId;
        var result = await _locationService.UnmarkLocationPopularAsync(id, userId, cancellationToken);
        return HandleResult(result);
    }

    // Harbors endpoints
    [HttpGet("{locationId}/harbors")]
    public async Task<IActionResult> GetLocationHarbors(Guid locationId, CancellationToken cancellationToken)
    {
        var result = await _harborService.GetLocationHarborsAsync(locationId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{locationId}/harbors")]
    public async Task<IActionResult> CreateHarbor(Guid locationId, [FromBody] CreateHarborDto dto, CancellationToken cancellationToken)
    {
        if (locationId != dto.LocationId) return HandleResult(Result.Failure("VALIDATION_ERROR", "LocationId eşleşmiyor."));

        var userId = this.UserId;
        var result = await _harborService.CreateHarborAsync(dto, userId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{locationId}/harbors/{id}")]
    public async Task<IActionResult> UpdateHarbor(Guid locationId, Guid harborId, [FromBody] UpdateHarborDto dto, CancellationToken cancellationToken)
    {
        if (locationId != dto.LocationId) return HandleResult(Result.Failure("VALIDATION_ERROR", "LocationId eşleşmiyor."));
        if (harborId != dto.Id) return HandleResult(Result.Failure("VALIDATION_ERROR", "Id eşleşmiyor."));

        var userId = this.UserId;
        var result = await _harborService.UpdateHarborAsync(dto, userId, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{locationId}/harbors/{id}")]
    public async Task<IActionResult> DeleteHarbor(Guid locationId, Guid harborId, CancellationToken cancellationToken)
    {
        var userId = this.UserId;
        var result = await _harborService.DeleteHarborAsync(harborId, userId, cancellationToken);
        return HandleResult(result);
    }
}
