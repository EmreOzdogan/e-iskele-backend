using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EIskele.Application.Dashboard;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Threading;

namespace EIskele.Api.Controllers.Captain;

[Authorize(Roles = "Captain")]
[ApiController]
[Route("api/captain/dashboard")]
public class CaptainDashboardController : ControllerBase
{
    private readonly ICaptainDashboardService _dashboardService;

    public CaptainDashboardController(ICaptainDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _dashboardService.GetDashboardSummaryAsync(userId, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { success = false, message = result.Error });

        return Ok(new { success = true, data = result.Value });
    }
}
