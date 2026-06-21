using System;
using System.Collections.Generic;

namespace EIskele.Application.Captains;

public class CaptainHubDocumentDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string RequirementLevel { get; set; } = string.Empty;
    public string Status { get; set; } = "notUploaded";
    
    public string? BoatId { get; set; }
    public string? BoatName { get; set; }
    
    public string? FileName { get; set; }
    public string? UploadedAtText { get; set; }
    public string? UpdatedAtText { get; set; }
    public string? ValidUntilText { get; set; }
    
    public string? AdminNote { get; set; }
    public string? RejectionReason { get; set; }
    
    public bool IsSensitive { get; set; } = true;
    public List<string> AllowedFileTypes { get; set; } = new() { "PDF", "JPG", "PNG", "JPEG" };
    public int MaxFileSizeMb { get; set; } = 10;
    
    public List<CaptainDocumentHistoryDto> History { get; set; } = new();
}

public class CaptainDocumentHistoryDto
{
    public string Id { get; set; } = string.Empty;
    public string DateText { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CaptainDocumentsSummaryDto
{
    public int Total { get; set; }
    public int Approved { get; set; }
    public int PendingReview { get; set; }
    public int NotUploaded { get; set; }
    public int NeedsUpdate { get; set; }
    public int Rejected { get; set; }
    public int ExpiringSoon { get; set; }
    public int CompletionRate { get; set; }
}

public class CaptainDocumentsDataDto
{
    public CaptainDocumentsSummaryDto Summary { get; set; } = new();
    public List<CaptainHubDocumentDto> Documents { get; set; } = new();
}

public class UploadCaptainDocumentRequest
{
    public string? Note { get; set; }
    public DateTime? ValidUntil { get; set; }
}
