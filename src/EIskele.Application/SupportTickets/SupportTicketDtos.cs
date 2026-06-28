using System;

namespace EIskele.Application.SupportTickets;

public class SupportTicketDto
{
    public Guid Id { get; set; }
    public string TicketNo { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateSupportTicketDto
{
    public string Subject { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
}

public class UpdateSupportTicketStatusDto
{
    public string Status { get; set; } = string.Empty;
}
