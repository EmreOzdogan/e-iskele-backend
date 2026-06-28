using EIskele.Application.Payouts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers.Captain;

[ApiController]
[Route("api/captain/payouts")]
[Authorize(Roles = "Captain")]
public class CaptainPayoutsController : BaseController
{
    private readonly IPayoutService _payoutService;

    public CaptainPayoutsController(IPayoutService payoutService)
    {
        _payoutService = payoutService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyPayouts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _payoutService.GetCaptainPayoutsAsync(UserId, page, pageSize, cancellationToken);
        return HandleResult(result);
    }
}
