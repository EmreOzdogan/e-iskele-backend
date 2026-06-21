using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Captains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EIskele.Api.Controllers;

[Route("api/[controller]")]
public class CaptainsController : BaseController
{
    private readonly ICaptainService _captainService;

    public CaptainsController(ICaptainService captainService)
    {
        _captainService = captainService;
    }

    [HttpPost("apply")]
    [AllowAnonymous]
    public async Task<IActionResult> Apply([FromBody] CaptainApplicationRequest request, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid.TryParse(userIdString, out var userId); // Defaults to Guid.Empty if not logged in

        var result = await _captainService.ApplyAsync(userId, request, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/approve")]
    // [Authorize(Roles = "Admin,SuperAdmin")] // Yorum satırı: auth eklendiğinde açılabilir
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var result = await _captainService.ApproveApplicationAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/reject")]
    // [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] string reason, CancellationToken cancellationToken)
    {
        var result = await _captainService.RejectApplicationAsync(id, reason, cancellationToken);
        return HandleResult(result);
    }
}
