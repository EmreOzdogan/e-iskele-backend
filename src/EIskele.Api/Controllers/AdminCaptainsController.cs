using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Captains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers;

[Route("api/admin/captains")]
// [Authorize(Roles = "Admin,SuperAdmin")] // Uncomment when auth is ready
public class AdminCaptainsController : BaseController
{
    private readonly ICaptainService _captainService;

    public AdminCaptainsController(ICaptainService captainService)
    {
        _captainService = captainService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var result = await _captainService.GetAdminCaptainsSummaryAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetCaptains([FromQuery] GetAdminCaptainsQuery query, CancellationToken cancellationToken)
    {
        var result = await _captainService.GetAdminCaptainsAsync(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCaptainDetail(Guid id, CancellationToken cancellationToken)
    {
        var result = await _captainService.GetAdminCaptainDetailAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> ApproveCaptain(Guid id, CancellationToken cancellationToken)
    {
        var result = await _captainService.ApproveApplicationAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> RejectCaptain(Guid id, [FromBody] string reason, CancellationToken cancellationToken)
    {
        var result = await _captainService.RejectApplicationAsync(id, reason, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/suspend")]
    public async Task<IActionResult> SuspendCaptain(Guid id, [FromBody] string reason, CancellationToken cancellationToken)
    {
        var result = await _captainService.SuspendCaptainAsync(id, reason, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/reactivate")]
    public async Task<IActionResult> ReactivateCaptain(Guid id, CancellationToken cancellationToken)
    {
        var result = await _captainService.ReactivateCaptainAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("documents/{documentId:guid}/approve")]
    public async Task<IActionResult> ApproveDocument(Guid documentId, CancellationToken cancellationToken)
    {
        var result = await _captainService.ApproveDocumentAsync(documentId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("documents/{documentId:guid}/reject")]
    public async Task<IActionResult> RejectDocument(Guid documentId, [FromBody] string reason, CancellationToken cancellationToken)
    {
        var result = await _captainService.RejectDocumentAsync(documentId, reason, cancellationToken);
        return HandleResult(result);
    }
}
