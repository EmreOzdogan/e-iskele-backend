using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EIskele.Application.Layout;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Threading;

namespace EIskele.Api.Controllers.Captain;

[Authorize(Roles = "Captain")]
[ApiController]
[Route("api/captain/layout")]
public class CaptainLayoutController : ControllerBase
{
    private readonly ICaptainLayoutService _layoutService;

    public CaptainLayoutController(ICaptainLayoutService layoutService)
    {
        _layoutService = layoutService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLayoutData(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _layoutService.GetLayoutDataAsync(userId, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { success = false, message = result.Error });

        return Ok(new { success = true, data = result.Value });
    }
}
