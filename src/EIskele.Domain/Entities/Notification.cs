using System;
using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public DateTime? SentAt { get; set; }
}
