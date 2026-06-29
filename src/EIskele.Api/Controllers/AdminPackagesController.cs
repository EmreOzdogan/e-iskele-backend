using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EIskele.Application.Packages;
using System.Threading.Tasks;
using System;

namespace EIskele.Api.Controllers;

[ApiController]
[Route("api/admin/packages")]
// [Authorize(Roles = "Admin,SuperAdmin")] // Uncomment when auth is fully setup
public class AdminPackagesController : BaseController
{
    private readonly ITourPackageService _packageService;

    public AdminPackagesController(ITourPackageService packageService)
    {
        _packageService = packageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPackages([FromQuery] string? status, [FromQuery] string? search, [FromQuery] Guid? boatId, CancellationToken cancellationToken)
    {
        var result = await _packageService.GetAdminPackagesAsync(status, search, boatId, cancellationToken);
        return HandleResult(result, "Paketler listelendi.");
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetPackageStats(CancellationToken cancellationToken)
    {
        var result = await _packageService.GetAdminPackageStatsAsync(cancellationToken);
        return HandleResult(result, "Paket istatistikleri getirildi.");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPackageDetail(Guid id)
    {
        var result = await _packageService.GetAdminPackageDetailAsync(id);
        return HandleResult(result, "Paket detayı getirildi.");
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApprovePackage(Guid id)
    {
        var result = await _packageService.ApprovePackageAsync(id);
        return HandleResult(result, "Paket onaylandı ve aktif edildi.");
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> RejectPackage(Guid id, [FromBody] RejectPackageRequest request)
    {
        var result = await _packageService.RejectPackageAsync(id, request.Reason);
        return HandleResult(result, "Paket reddedildi.");
    }

    [HttpPost("{id}/request-revision")]
    public async Task<IActionResult> RequestRevision(Guid id, [FromBody] RequestRevisionPackageRequest request)
    {
        var result = await _packageService.RequestRevisionAsync(id, request.Fields, request.Note);
        return HandleResult(result, "Düzenleme talebi oluşturuldu.");
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivatePackage(Guid id, [FromBody] RejectPackageRequest request)
    {
        var result = await _packageService.DeactivatePackageAsync(id, request.Reason);
        return HandleResult(result, "Paket pasife alındı.");
    }

    [HttpPost("{id}/suspend")]
    public async Task<IActionResult> SuspendPackage(Guid id, [FromBody] RejectPackageRequest request)
    {
        var result = await _packageService.SuspendPackageAsync(id, request.Reason);
        return HandleResult(result, "Paket askıya alındı.");
    }

    [HttpPost("{id}/reactivate")]
    public async Task<IActionResult> ReactivatePackage(Guid id)
    {
        var result = await _packageService.ReactivatePackageAsync(id);
        return HandleResult(result, "Paket tekrar aktif edildi.");
    }
}

public class RejectPackageRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class RequestRevisionPackageRequest
{
    public string[] Fields { get; set; } = Array.Empty<string>();
    public string Note { get; set; } = string.Empty;
}
