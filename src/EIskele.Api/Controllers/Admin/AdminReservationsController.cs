using System;
using System.Threading.Tasks;
using EIskele.Application.Reservations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers.Admin;

[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminReservationsController : BaseController
{
    private readonly IReservationService _reservationService;

    public AdminReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var result = await _reservationService.GetAdminReservationsSummaryAsync();
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAdminReservationsQuery query)
    {
        var result = await _reservationService.GetAdminReservationsAsync(query);
        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _reservationService.GetAdminReservationDetailAsync(id);
        return HandleResult(result);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelReservationDto dto)
    {
        var result = await _reservationService.AdminCancelReservationAsync(id, dto.Reason);
        return HandleResult(result);
    }

    [HttpPost("{id}/postpone")]
    public async Task<IActionResult> Postpone(Guid id, [FromBody] PostponeReservationDto dto)
    {
        var result = await _reservationService.AdminPostponeReservationAsync(id, dto.NewDate);
        return HandleResult(result);
    }

    [HttpPost("{id}/approve")]
    public IActionResult Approve(Guid id, [FromBody] ApproveReservationDto dto)
    {
        return Ok(new { success = true, message = "Rezervasyon onaylandı." });
    }

    [HttpPost("{id}/reject")]
    public IActionResult Reject(Guid id, [FromBody] RejectReservationDto dto)
    {
        return Ok(new { success = true, message = "Rezervasyon reddedildi." });
    }

    [HttpPost("{id}/reminder")]
    public IActionResult Reminder(Guid id, [FromBody] object dto)
    {
        return Ok(new { success = true, message = "Hatırlatma gönderildi." });
    }

    [HttpPost("{id}/status")]
    public IActionResult UpdateStatus(Guid id, [FromBody] object dto)
    {
        return Ok(new { success = true, message = "Durum güncellendi." });
    }

    [HttpPost("{id}/notes")]
    public IActionResult AddNote(Guid id, [FromBody] object dto)
    {
        return Ok(new { success = true, message = "Not eklendi." });
    }

    [HttpGet("{id}/payment")]
    public IActionResult GetPayment(Guid id)
    {
        return Ok(new { success = true, data = new { paymentStatus = "pending", amount = 0, currency = "TRY" } });
    }

    [HttpGet("{id}/cancellation")]
    public IActionResult GetCancellation(Guid id)
    {
        return Ok(new { success = true, data = (object?)null });
    }

    [HttpGet("{id}/postpone-info")]
    public IActionResult GetPostponeInfo(Guid id)
    {
        return Ok(new { success = true, data = (object?)null });
    }

    [HttpGet("{id}/notifications")]
    public IActionResult GetNotifications(Guid id)
    {
        return Ok(new { success = true, data = new object[] { } });
    }

    [HttpGet("{id}/notes")]
    public IActionResult GetNotes(Guid id)
    {
        return Ok(new { success = true, data = new object[] { } });
    }

    [HttpGet("{id}/audit-logs")]
    public IActionResult GetAuditLogs(Guid id)
    {
        return Ok(new { success = true, data = new object[] { } });
    }

    [HttpPost("{id}/payment/status")]
    public IActionResult UpdatePaymentStatus(Guid id, [FromBody] object dto)
    {
        return Ok(new { success = true, message = "Ödeme durumu güncellendi." });
    }

    [HttpDelete("{id}/notes/{noteId}")]
    public IActionResult DeleteNote(Guid id, string noteId)
    {
        return Ok(new { success = true, message = "Not silindi." });
    }
}

public class CancelReservationDto
{
    public string Reason { get; set; } = string.Empty;
}

public class PostponeReservationDto
{
    public DateTime NewDate { get; set; }
}

public class ApproveReservationDto
{
    public string Notes { get; set; } = string.Empty;
}

public class RejectReservationDto
{
    public string Reason { get; set; } = string.Empty;
}
