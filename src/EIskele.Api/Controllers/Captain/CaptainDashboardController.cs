using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EIskele.Application.Dashboard;
using System.Threading.Tasks;
using System.Threading;

namespace EIskele.Api.Controllers.Captain;

[Authorize(Roles = "Captain")]
[Route("api/captain/dashboard")]
public class CaptainDashboardController : BaseController
{
    private readonly ICaptainDashboardService _dashboardService;

    public CaptainDashboardController(ICaptainDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        if (this.UserId == System.Guid.Empty)
            return HandleResult(EIskele.Application.Common.Results.Result.Failure("UNAUTHORIZED", "Kullanıcı doğrulanamadı."));

        var result = await _dashboardService.GetDashboardSummaryAsync(this.UserId.ToString(), cancellationToken);
        return HandleResult(result);
    }
}
