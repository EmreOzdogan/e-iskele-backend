using EIskele.Application.Payouts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/payouts")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminPayoutsController : BaseController
{
    private readonly IPayoutService _payoutService;

    public AdminPayoutsController(IPayoutService payoutService)
    {
        _payoutService = payoutService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPayouts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _payoutService.GetAdminPayoutsAsync(page, pageSize, search, status, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdatePayoutStatus(
        Guid id,
        [FromBody] UpdatePayoutStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        var result = await _payoutService.UpdatePayoutStatusAsync(id, dto, UserId, cancellationToken);
        return HandleResult(result);
    }
}
