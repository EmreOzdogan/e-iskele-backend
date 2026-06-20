using System;
using System.Threading.Tasks;
using EIskele.Application.Reservations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers.Captain;

[Route("api/captain/[controller]")]
[Authorize(Roles = "Captain")]
public class ReservationsController : BaseController
{
    private readonly ICaptainReservationService _reservationService;

    public ReservationsController(ICaptainReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetReservations([FromQuery] string status = "all")
    {
        var result = await _reservationService.GetReservationsAsync(UserId, status);
        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReservationDetail(Guid id)
    {
        var result = await _reservationService.GetReservationDetailAsync(UserId, id);
        return HandleResult(result);
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveReservation(Guid id, [FromBody] ApproveCaptainReservationRequest request)
    {
        var result = await _reservationService.ApproveReservationAsync(UserId, id, request.Note);
        return HandleResult(result);
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> RejectReservation(Guid id, [FromBody] RejectCaptainReservationRequest request)
    {
        var result = await _reservationService.RejectReservationAsync(UserId, id, request.Reason);
        return HandleResult(result);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelReservation(Guid id, [FromBody] RejectCaptainReservationRequest request)
    {
        var result = await _reservationService.CancelReservationAsync(UserId, id, request.Reason);
        return HandleResult(result);
    }

    [HttpPatch("{id}/note")]
    public async Task<IActionResult> UpdateCaptainNote(Guid id, [FromBody] UpdateCaptainReservationNoteRequest request)
    {
        var result = await _reservationService.UpdateCaptainNoteAsync(UserId, id, request.Note);
        return HandleResult(result);
    }
}
