using System;

namespace EIskele.Infrastructure.Emails.Models;

public sealed class TestEmailModel : BaseEmailTemplateModel
{
    public string AdminPanelUrl { get; set; } = string.Empty;
}
