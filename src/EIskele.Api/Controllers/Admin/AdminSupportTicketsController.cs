using EIskele.Application.SupportTickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/support-tickets")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminSupportTicketsController : BaseController
{
    private readonly ISupportTicketService _supportTicketService;

    public AdminSupportTicketsController(ISupportTicketService supportTicketService)
    {
        _supportTicketService = supportTicketService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTickets(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _supportTicketService.GetAdminTicketsAsync(page, pageSize, search, status, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateTicketStatus(
        Guid id,
        [FromBody] UpdateSupportTicketStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        var result = await _supportTicketService.UpdateTicketStatusAsync(id, dto, UserId, cancellationToken);
        return HandleResult(result);
    }
}
