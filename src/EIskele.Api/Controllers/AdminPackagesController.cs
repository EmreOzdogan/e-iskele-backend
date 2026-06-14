using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EIskele.Application.Packages;
using System.Threading.Tasks;
using System;

namespace EIskele.Api.Controllers;

[ApiController]
[Route("api/admin/packages")]
// [Authorize(Roles = "Admin,SuperAdmin")] // Uncomment when auth is fully setup
public class AdminPackagesController : ControllerBase
{
    private readonly ITourPackageService _packageService;

    public AdminPackagesController(ITourPackageService packageService)
    {
        _packageService = packageService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPackageDetail(Guid id)
    {
        var result = await _packageService.GetAdminPackageDetailAsync(id);
        if (result.IsSuccess) return Ok(new { success = true, data = result.Value, message = "Paket detayı getirildi." });
        return NotFound(new { success = false, message = result.Error.Message });
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApprovePackage(Guid id)
    {
        var result = await _packageService.ApprovePackageAsync(id);
        if (result.IsSuccess) return Ok(new { success = true, message = "Paket onaylandı ve aktif edildi." });
        return BadRequest(new { success = false, message = result.Error.Message });
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> RejectPackage(Guid id, [FromBody] RejectPackageRequest request)
    {
        var result = await _packageService.RejectPackageAsync(id, request.Reason);
        if (result.IsSuccess) return Ok(new { success = true, message = "Paket reddedildi." });
        return BadRequest(new { success = false, message = result.Error.Message });
    }

    [HttpPost("{id}/request-revision")]
    public async Task<IActionResult> RequestRevision(Guid id, [FromBody] RequestRevisionPackageRequest request)
    {
        var result = await _packageService.RequestRevisionAsync(id, request.Fields, request.Note);
        if (result.IsSuccess) return Ok(new { success = true, message = "Düzenleme talebi oluşturuldu." });
        return BadRequest(new { success = false, message = result.Error.Message });
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivatePackage(Guid id, [FromBody] RejectPackageRequest request)
    {
        var result = await _packageService.DeactivatePackageAsync(id, request.Reason);
        if (result.IsSuccess) return Ok(new { success = true, message = "Paket pasife alındı." });
        return BadRequest(new { success = false, message = result.Error.Message });
    }

    [HttpPost("{id}/suspend")]
    public async Task<IActionResult> SuspendPackage(Guid id, [FromBody] RejectPackageRequest request)
    {
        var result = await _packageService.SuspendPackageAsync(id, request.Reason);
        if (result.IsSuccess) return Ok(new { success = true, message = "Paket askıya alındı." });
        return BadRequest(new { success = false, message = result.Error.Message });
    }

    [HttpPost("{id}/reactivate")]
    public async Task<IActionResult> ReactivatePackage(Guid id)
    {
        var result = await _packageService.ReactivatePackageAsync(id);
        if (result.IsSuccess) return Ok(new { success = true, message = "Paket tekrar aktif edildi." });
        return BadRequest(new { success = false, message = result.Error.Message });
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
