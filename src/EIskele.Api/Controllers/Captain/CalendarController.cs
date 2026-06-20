using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Availability;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers.Captain;

[Authorize(Roles = "Captain")]
[Route("api/[controller]")]
public class CalendarController : BaseController
{
    private readonly ICaptainCalendarService _calendarService;

    public CalendarController(ICaptainCalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    [HttpGet("metrics")]
    public async Task<IActionResult> GetMetrics(CancellationToken cancellationToken)
    {
        var result = await _calendarService.GetMetricsAsync(UserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("slots")]
    public async Task<IActionResult> GetSlots([FromQuery] CaptainCalendarFilterDto filters, CancellationToken cancellationToken)
    {
        var result = await _calendarService.GetSlotsAsync(UserId, filters, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("slots")]
    public async Task<IActionResult> AddSlot([FromBody] AddAvailabilityBlockRequest request, CancellationToken cancellationToken)
    {
        var result = await _calendarService.AddAvailabilityBlockAsync(UserId, request, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("slots/{id}")]
    public async Task<IActionResult> DeleteSlot(Guid id, CancellationToken cancellationToken)
    {
        var result = await _calendarService.DeleteAvailabilityBlockAsync(UserId, id, cancellationToken);
        return HandleResult(result);
    }
}
