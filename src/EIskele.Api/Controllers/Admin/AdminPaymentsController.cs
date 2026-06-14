using Microsoft.AspNetCore.Mvc;
using EIskele.Application.Payments;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using EIskele.Domain.Enums;

namespace EIskele.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/payments")]
[Authorize(Roles = "Admin,SuperAdmin")] // Assume role check is handled
public class AdminPaymentsController : BaseController
{
    private readonly IPaymentService _paymentService;

    public AdminPaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet("metrics")]
    public async Task<IActionResult> GetSummaryMetrics()
    {
        var result = await _paymentService.GetAdminPaymentsSummaryAsync();
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetAdminPaymentsQuery query)
    {
        var result = await _paymentService.GetAdminPaymentsAsync(query);
        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var result = await _paymentService.GetAdminPaymentDetailAsync(id);
        return HandleResult(result);
    }

    [HttpPost("{id}/refund")]
    public async Task<IActionResult> ProcessRefund(Guid id, [FromBody] RefundRequestDto request)
    {
        var result = await _paymentService.ProcessRefundRequestAsync(id, request.Action, request.Reason);
        return HandleResult(result);
    }

    [HttpPatch("{id}/payout-status")]
    public async Task<IActionResult> UpdatePayoutStatus(Guid id, [FromBody] PayoutStatusUpdateDto request)
    {
        var result = await _paymentService.UpdatePayoutStatusAsync(id, request.Status, request.Reason);
        return HandleResult(result);
    }
}

public class RefundRequestDto
{
    public string Action { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

public class PayoutStatusUpdateDto
{
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
