using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.SupportTickets;

public interface ISupportTicketService
{
    Task<Result<PagedResult<SupportTicketDto>>> GetAdminTicketsAsync(int page = 1, int pageSize = 20, string? search = null, string? status = null, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<SupportTicketDto>>> GetMyTicketsAsync(Guid userId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<Result<SupportTicketDto>> CreateTicketAsync(CreateSupportTicketDto dto, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> UpdateTicketStatusAsync(Guid id, UpdateSupportTicketStatusDto dto, Guid adminId, CancellationToken cancellationToken = default);
}
