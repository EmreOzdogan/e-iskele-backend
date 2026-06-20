using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EIskele.Application.Earnings;
using EIskele.Api.Controllers;

namespace EIskele.Api.Controllers.Captain;

[Authorize(Roles = "Captain")]
[ApiController]
[Route("api/captain/earnings")]
public class CaptainEarningsController : BaseController
{
    private readonly ICaptainEarningService _earningService;

    public CaptainEarningsController(ICaptainEarningService earningService)
    {
        _earningService = earningService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var result = await _earningService.GetSummaryAsync(UserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] string period = "all", CancellationToken cancellationToken = default)
    {
        var result = await _earningService.GetHistoryAsync(UserId, period, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("payouts")]
    public async Task<IActionResult> GetPayouts([FromQuery] string status = "all", CancellationToken cancellationToken = default)
    {
        var result = await _earningService.GetPayoutsAsync(UserId, status, cancellationToken);
        return HandleResult(result);
    }
}
