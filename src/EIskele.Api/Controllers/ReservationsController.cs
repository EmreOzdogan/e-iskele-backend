using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Reservations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers;

[Route("api/[controller]")]
public class ReservationsController : BaseController
{
    private readonly IReservationCommandService _reservationCommandService;
    private readonly IReservationQueryService _reservationQueryService;

    public ReservationsController(
        IReservationCommandService reservationCommandService,
        IReservationQueryService reservationQueryService)
    {
        _reservationCommandService = reservationCommandService;
        _reservationQueryService = reservationQueryService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequest request, CancellationToken cancellationToken)
    {
        var result = await _reservationCommandService.CreateReservationAsync(request, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> ApproveReservation(Guid id, CancellationToken cancellationToken)
    {
        var result = await _reservationCommandService.ApproveReservationAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelReservation(Guid id, [FromBody] string reason, CancellationToken cancellationToken)
    {
        var result = await _reservationCommandService.CancelReservationAsync(id, reason, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("users/{userId:guid}")]
    public async Task<IActionResult> GetUserReservations(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _reservationQueryService.GetReservationsByUserIdAsync(userId, cancellationToken);
        return HandleResult(result);
    }
}
