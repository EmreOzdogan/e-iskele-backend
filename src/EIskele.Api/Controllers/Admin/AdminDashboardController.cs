using Microsoft.AspNetCore.Mvc;
using EIskele.Application.Dashboard;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace EIskele.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = "Admin,SuperAdmin")] // Adjust as per your setup
public class AdminDashboardController : BaseController
{
    private readonly IDashboardService _dashboardService;

    public AdminDashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSummary()
    {
        var result = await _dashboardService.GetAdminDashboardSummaryAsync();
        return HandleResult(result);
    }
}
