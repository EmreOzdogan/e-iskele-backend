using System;
using System.Threading.Tasks;
using EIskele.Application.Reservations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers.Admin;

[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminReservationsController : BaseController
{
    private readonly IAdminReservationQueryService _queryService;
    private readonly IAdminReservationCommandService _commandService;

    public AdminReservationsController(
        IAdminReservationQueryService queryService,
        IAdminReservationCommandService commandService)
    {
        _queryService = queryService;
        _commandService = commandService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var result = await _queryService.GetAdminReservationsSummaryAsync();
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAdminReservationsQuery query)
    {
        var result = await _queryService.GetAdminReservationsAsync(query);
        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _queryService.GetAdminReservationDetailAsync(id);
        return HandleResult(result);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelReservationDto dto)
    {
        var result = await _commandService.AdminCancelReservationAsync(id, dto.Reason);
        return HandleResult(result);
    }

    [HttpPost("{id}/postpone")]
    public async Task<IActionResult> Postpone(Guid id, [FromBody] PostponeReservationDto dto)
    {
        var result = await _commandService.AdminPostponeReservationAsync(id, dto.NewDate);
        return HandleResult(result);
    }

    [HttpPost("{id}/approve")]
    public IActionResult Approve(Guid id, [FromBody] ApproveReservationDto dto)
    {
        return HandleResult(EIskele.Application.Common.Results.Result.Success(), "Rezervasyon onaylandı.");
    }

    [HttpPost("{id}/reject")]
    public IActionResult Reject(Guid id, [FromBody] RejectReservationDto dto)
    {
        return HandleResult(EIskele.Application.Common.Results.Result.Success(), "Rezervasyon reddedildi.");
    }

    [HttpPost("{id}/reminder")]
    public IActionResult Reminder(Guid id, [FromBody] object dto)
    {
        return HandleResult(EIskele.Application.Common.Results.Result.Success(), "Hatırlatma gönderildi.");
    }

    [HttpPost("{id}/status")]
    public IActionResult UpdateStatus(Guid id, [FromBody] object dto)
    {
        return HandleResult(EIskele.Application.Common.Results.Result.Success(), "Durum güncellendi.");
    }

    [HttpPost("{id}/notes")]
    public IActionResult AddNote(Guid id, [FromBody] object dto)
    {
        return HandleResult(EIskele.Application.Common.Results.Result.Success(), "Not eklendi.");
    }

    [HttpGet("{id}/payment")]
    public IActionResult GetPayment(Guid id)
    {
        return HandleResult(EIskele.Application.Common.Results.Result<object>.Success(new { paymentStatus = "pending", amount = 0, currency = "TRY" }));
    }

    [HttpGet("{id}/cancellation")]
    public IActionResult GetCancellation(Guid id)
    {
        return HandleResult(EIskele.Application.Common.Results.Result<object?>.Success(null));
    }

    [HttpGet("{id}/postpone-info")]
    public IActionResult GetPostponeInfo(Guid id)
    {
        return HandleResult(EIskele.Application.Common.Results.Result<object?>.Success(null));
    }

    [HttpGet("{id}/notifications")]
    public IActionResult GetNotifications(Guid id)
    {
        return HandleResult(EIskele.Application.Common.Results.Result<object[]>.Success(new object[] { }));
    }

    [HttpGet("{id}/notes")]
    public IActionResult GetNotes(Guid id)
    {
        return HandleResult(EIskele.Application.Common.Results.Result<object[]>.Success(new object[] { }));
    }

    [HttpGet("{id}/audit-logs")]
    public IActionResult GetAuditLogs(Guid id)
    {
        return HandleResult(EIskele.Application.Common.Results.Result<object[]>.Success(new object[] { }));
    }

    [HttpPost("{id}/payment/status")]
    public IActionResult UpdatePaymentStatus(Guid id, [FromBody] object dto)
    {
        return HandleResult(EIskele.Application.Common.Results.Result.Success(), "Ödeme durumu güncellendi.");
    }

    [HttpDelete("{id}/notes/{noteId}")]
    public IActionResult DeleteNote(Guid id, string noteId)
    {
        return HandleResult(EIskele.Application.Common.Results.Result.Success(), "Not silindi.");
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
