using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Boats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers;

[Authorize(Roles = "Admin,SuperAdmin")]
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBoatDetail(Guid id, CancellationToken cancellationToken)
    {
        var result = await _boatService.GetAdminBoatDetailAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id}/images")]
    public async Task<IActionResult> GetBoatImages(Guid id, CancellationToken cancellationToken)
    {
        var result = await _boatService.GetAdminBoatImagesAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id}/documents")]
    public async Task<IActionResult> GetBoatDocuments(Guid id, CancellationToken cancellationToken)
    {
        var result = await _boatService.GetAdminBoatDocumentsAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id}/features")]
    public async Task<IActionResult> GetBoatFeatures(Guid id, CancellationToken cancellationToken)
    {
        var result = await _boatService.GetAdminBoatFeaturesAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id}/packages")]
    public async Task<IActionResult> GetBoatPackages(Guid id, CancellationToken cancellationToken)
    {
        var result = await _boatService.GetAdminBoatPackagesAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveBoat(Guid id, CancellationToken cancellationToken)
    {
        var result = await _boatService.ApproveBoatAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> RejectBoat(Guid id, [FromBody] RejectRequest request, CancellationToken cancellationToken)
    {
        var result = await _boatService.RejectBoatAsync(id, request.Reason, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id}/request-revision")]
    public async Task<IActionResult> RequestRevision(Guid id, [FromBody] RevisionRequest request, CancellationToken cancellationToken)
    {
        var result = await _boatService.RequestBoatRevisionAsync(id, request.Fields, request.Note, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateBoat(Guid id, [FromBody] ReasonRequest request, CancellationToken cancellationToken)
    {
        var result = await _boatService.DeactivateBoatAsync(id, request.Reason, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id}/suspend")]
    public async Task<IActionResult> SuspendBoat(Guid id, [FromBody] ReasonRequest request, CancellationToken cancellationToken)
    {
        var result = await _boatService.SuspendBoatAsync(id, request.Reason, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id}/reactivate")]
    public async Task<IActionResult> ReactivateBoat(Guid id, CancellationToken cancellationToken)
    {
        var result = await _boatService.ReactivateBoatAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id}/images/{imageId}/approve")]
    public async Task<IActionResult> ApproveBoatImage(Guid id, Guid imageId, CancellationToken cancellationToken)
    {
        var result = await _boatService.ApproveBoatImageAsync(imageId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id}/images/{imageId}/reject")]
    public async Task<IActionResult> RejectBoatImage(Guid id, Guid imageId, [FromBody] ReasonRequest request, CancellationToken cancellationToken)
    {
        var result = await _boatService.RejectBoatImageAsync(imageId, request.Reason, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id}/documents/{documentId}/approve")]
    public async Task<IActionResult> ApproveBoatDocument(Guid id, Guid documentId, CancellationToken cancellationToken)
    {
        var result = await _boatService.ApproveBoatDocumentAsync(documentId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id}/documents/{documentId}/reject")]
    public async Task<IActionResult> RejectBoatDocument(Guid id, Guid documentId, [FromBody] ReasonRequest request, CancellationToken cancellationToken)
    {
        var result = await _boatService.RejectBoatDocumentAsync(documentId, request.Reason, cancellationToken);
        return HandleResult(result);
    }
}

public class RejectRequest { public string Reason { get; set; } = string.Empty; }
public class ReasonRequest { public string Reason { get; set; } = string.Empty; }
public class RevisionRequest { public System.Collections.Generic.List<string> Fields { get; set; } = new(); public string Note { get; set; } = string.Empty; }
