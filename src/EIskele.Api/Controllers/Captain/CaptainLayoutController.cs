using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EIskele.Application.Layout;
using System.Threading.Tasks;
using System.Threading;

namespace EIskele.Api.Controllers.Captain;

[Authorize(Roles = "Captain")]
[Route("api/captain/layout")]
public class CaptainLayoutController : BaseController
{
    private readonly ICaptainLayoutService _layoutService;

    public CaptainLayoutController(ICaptainLayoutService layoutService)
    {
        _layoutService = layoutService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLayoutData(CancellationToken cancellationToken)
    {
        if (this.UserId == System.Guid.Empty)
            return Unauthorized();

        var result = await _layoutService.GetLayoutDataAsync(this.UserId.ToString(), cancellationToken);
        return HandleResult(result);
    }
}
