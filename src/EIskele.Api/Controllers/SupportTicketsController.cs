using EIskele.Application.SupportTickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers;

[ApiController]
[Route("api/support-tickets")]
[Authorize]
public class SupportTicketsController : BaseController
{
    private readonly ISupportTicketService _supportTicketService;

    public SupportTicketsController(ISupportTicketService supportTicketService)
    {
        _supportTicketService = supportTicketService;
    }

    [HttpGet("my-tickets")]
    public async Task<IActionResult> GetMyTickets(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _supportTicketService.GetMyTicketsAsync(UserId, page, pageSize, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTicket(
        [FromBody] CreateSupportTicketDto dto,
        CancellationToken cancellationToken = default)
    {
        var result = await _supportTicketService.CreateTicketAsync(dto, UserId, cancellationToken);
        return HandleResult(result);
    }
}
