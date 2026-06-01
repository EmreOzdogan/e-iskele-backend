using System;
using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class NotificationTemplate : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string SubjectTemplate { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
