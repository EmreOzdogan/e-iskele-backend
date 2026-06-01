using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Availability;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers;

[Route("api/[controller]")]
public class AvailabilityController : BaseController
{
    private readonly IAvailabilityService _availabilityService;

    public AvailabilityController(IAvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
    }

    [HttpGet("check")]
    public async Task<IActionResult> Check([FromQuery] CheckAvailabilityRequest request, CancellationToken cancellationToken)
    {
        var result = await _availabilityService.CheckAvailabilityAsync(request, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("block")]
    public async Task<IActionResult> BlockDates([FromBody] BlockDatesRequest request, CancellationToken cancellationToken)
    {
        var result = await _availabilityService.BlockDatesAsync(request, cancellationToken);
        return HandleResult(result);
    }
}
