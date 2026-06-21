using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Captains;
using EIskele.Application.Common.Results;
using EIskele.Domain.Entities;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Services;

public partial class CaptainService
{
    public async Task<Result<AdminCaptainsSummaryDto>> GetAdminCaptainsSummaryAsync(CancellationToken cancellationToken = default)
    {
        var captains = await _dbContext.Captains.ToListAsync(cancellationToken);

        return Result<AdminCaptainsSummaryDto>.Success(new AdminCaptainsSummaryDto
        {
            TotalCaptains = captains.Count,
            ApprovedCaptains = captains.Count(c => c.Status == "Approved"),
            PendingApplications = captains.Count(c => c.Status == "UnderReview"),
            MissingDocuments = captains.Count(c => c.Status == "MissingDocument"),
            SuspendedCaptains = captains.Count(c => c.AccountStatus == "Suspended"),
            CompanyApplications = captains.Count(c => c.ApplicationType == "Company"),
            IndividualApplications = captains.Count(c => c.ApplicationType == "Individual"),
            NewApplicationsThisMonth = captains.Count(c => c.CreatedAt >= DateTime.UtcNow.AddMonths(-1))
        });
    }

    public async Task<Result<PagedResult<AdminCaptainListItemDto>>> GetAdminCaptainsAsync(GetAdminCaptainsQuery query, CancellationToken cancellationToken = default)
    {
        var q = _dbContext.Captains
            .Include(c => c.User)
            .Include(c => c.Boats)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            q = q.Where(c => c.ApplicationNo.ToLower().Contains(search) ||
                             c.User.FirstName.ToLower().Contains(search) ||
                             c.User.LastName.ToLower().Contains(search) ||
                             c.User.Email!.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.ApplicationStatus))
        {
            var appStatus = query.ApplicationStatus.ToLower();
            if (appStatus == "inreview")
                q = q.Where(c => c.Status == "UnderReview");
            else if (appStatus == "missingdocument")
                q = q.Where(c => c.Status == "MissingDocument");
            else if (appStatus == "suspended")
                q = q.Where(c => c.AccountStatus == "Suspended");
            else
                q = q.Where(c => c.Status.ToLower() == appStatus);
        }

        if (!string.IsNullOrWhiteSpace(query.CaptainStatus))
        {
            q = q.Where(c => c.AccountStatus.Equals(query.CaptainStatus, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.ApplicationType))
        {
            q = q.Where(c => c.ApplicationType.Equals(query.ApplicationType, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = await q.CountAsync(cancellationToken);

        var items = await q
            .OrderByDescending(c => c.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new AdminCaptainListItemDto
            {
                Id = c.Id,
                ApplicationNo = string.IsNullOrEmpty(c.ApplicationNo) ? $"APP-{c.Id.ToString().Substring(0,6).ToUpper()}" : c.ApplicationNo,
                DisplayName = $"{c.User.FirstName} {c.User.LastName}",
                ApplicationType = string.IsNullOrEmpty(c.ApplicationType) ? "individual" : c.ApplicationType.ToLower(),
                Email = c.User.Email ?? string.Empty,
                Phone = c.User.PhoneNumber ?? string.Empty,
                Location = c.Location,
                Harbor = c.Harbor,
                TotalBoatCount = c.Boats.Count,
                ActiveBoatCount = c.Boats.Count(b => b.Status == EIskele.Domain.Enums.BoatStatus.Published),
                DocumentStatus = "", // Will be populated dynamically below
                ApplicationStatus = c.Status == "UnderReview" ? "inReview" : c.Status == "MissingDocument" ? "missingDocument" : c.Status.ToLower(),
                AccountStatus = c.AccountStatus.ToLower(),
                AverageRating = 0, // Mocked for now
                TotalReservationCount = 0, // Mocked for now
                CompletedReservationCount = 0, // Mocked for now
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var captainIds = items.Select(x => x.Id.ToString()).ToList();
        var files = await _dbContext.StoredFiles
            .Where(f => f.RelatedEntityType == "CaptainDocument" && captainIds.Contains(f.RelatedEntityId) && !f.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            var captainFiles = files.Where(f => f.RelatedEntityId == item.Id.ToString()).ToList();
            int requiredDocCount = (item.ApplicationType == "company" ? 4 : 3) + (item.TotalBoatCount * 2);

            if (captainFiles.Count < requiredDocCount)
                item.DocumentStatus = "missing";
            else if (captainFiles.Any(f => f.Status == "rejected"))
                item.DocumentStatus = "rejected";
            else if (captainFiles.Any(f => f.Status == "pendingReview" || string.IsNullOrEmpty(f.Status) || f.Status == "uploaded"))
                item.DocumentStatus = "pendingReview";
            else if (captainFiles.Any(f => f.Status == "expiringSoon"))
                item.DocumentStatus = "expiringSoon";
            else
                item.DocumentStatus = "completed";
        }

        var result = new PagedResult<AdminCaptainListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };

        return Result<PagedResult<AdminCaptainListItemDto>>.Success(result);
    }

    public async Task<Result<AdminCaptainDetailDto>> GetAdminCaptainDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var c = await _dbContext.Captains
            .Include(x => x.User)
            .Include(x => x.Company)
            .Include(x => x.Boats)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (c == null)
            return Result<AdminCaptainDetailDto>.Failure("CAPTAIN_NOT_FOUND", "Kaptan bulunamadı.");

        // Stored files related to captain document
        var storedFiles = await _dbContext.StoredFiles
            .Where(f => f.RelatedEntityType == "CaptainDocument" && f.RelatedEntityId == id.ToString() && !f.IsDeleted)
            .ToListAsync(cancellationToken);

        var auditLogs = await _dbContext.AuditLogs
            .Where(a => a.ActorUserId == c.UserId || a.Description.Contains(c.Id.ToString()))
            .OrderByDescending(a => a.CreatedAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        var detail = new AdminCaptainDetailDto
        {
            Id = c.Id,
            ApplicationNo = string.IsNullOrEmpty(c.ApplicationNo) ? $"APP-{c.Id.ToString().Substring(0,6).ToUpper()}" : c.ApplicationNo,
            ApplicationType = string.IsNullOrEmpty(c.ApplicationType) ? "individual" : c.ApplicationType.ToLower(),
            DisplayName = $"{c.User.FirstName} {c.User.LastName}",
            Email = c.User.Email ?? string.Empty,
            Phone = c.User.PhoneNumber ?? string.Empty,
            Location = c.Location,
            Harbor = c.Harbor,
            ApplicationStatus = c.Status == "UnderReview" ? "inReview" : c.Status == "MissingDocument" ? "missingDocument" : c.Status.ToLower(),
            AccountStatus = c.AccountStatus.ToLower(),
            DocumentSummaryStatus = "completed",
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            AverageRating = 0,
            TotalReservations = 0,
            CompletedReservations = 0,
            
            Boats = c.Boats.Select(b => new CaptainBoatSummaryDto
            {
                Id = b.Id,
                BoatName = b.Name,
                Location = b.LocationId.ToString(), // Might need real location name lookup if needed
                Capacity = b.Capacity,
                ActivePackageCount = 0, // Needs tour package count
                BoatStatus = b.Status.ToString().ToLower(),
                DocumentStatus = "completed",
                PublishStatus = b.Status == EIskele.Domain.Enums.BoatStatus.Published ? "published" : "draft",
                UpdatedAt = b.UpdatedAt
            }).ToList(),

            AdminNotes = new List<CaptainAdminNoteDto>(), // We don't have separate entity for admin notes yet, just string

            AuditLogs = auditLogs.Select(a => new CaptainAuditLogDto
            {
                Id = a.Id,
                ActionDisplay = a.Action,
                Details = a.Description,
                CreatedBy = "System", // Or user ID
                CreatedAt = a.CreatedAt,
                Status = "success"
            }).ToList(),

            Documents = storedFiles.Select(f => 
            {
                var (docTitle, docType) = GetDocumentInfo(f.FileType);
                return new CaptainDocumentDto
                {
                    Id = f.Id,
                    DocumentType = docType,
                    Title = docTitle,
                    FileName = f.OriginalFileName,
                    FileSizeText = $"{Math.Round(f.SizeInBytes / 1024.0, 2)} KB",
                    UploadedAt = f.CreatedAt,
                    Status = string.IsNullOrEmpty(f.Status) ? "completed" : (f.Status == "approved" ? "completed" : f.Status)
                };
            }).ToList(),
            
            PerformanceMetrics = new CaptainPerformanceMetricsDto
            {
                RevenueRank = 1,
                ResponseRateText = "Hızlı Dönüş (%98)",
                CancellationRate = 0,
                CompletionRate = 100,
                AverageRating = 0,
                TotalReviews = 0
            }
        };

        if (detail.ApplicationType == "individual")
        {
            detail.IndividualInfo = new CaptainIndividualInfoDto
            {
                FullName = $"{c.User.FirstName} {c.User.LastName}",
                IdentityNumberMasked = MaskString(c.IdentityNumber, 2, 2),
                Address = c.Address,
                IbanMasked = MaskString(c.Iban, 4, 4)
            };
        }
        else if (c.Company != null)
        {
            detail.CompanyInfo = new CaptainCompanyInfoDto
            {
                CompanyName = c.Company.CompanyName,
                AuthorizedPersonName = c.Company.AuthorizedPersonName,
                TaxOffice = c.Company.TaxOffice,
                TaxNumberMasked = MaskString(c.Company.TaxNumber, 2, 2),
                Address = c.Company.Address,
                IbanMasked = MaskString(c.Company.Iban, 4, 4)
            };
        }

        if (!string.IsNullOrEmpty(c.AdminNote))
        {
            detail.AdminNotes.Add(new CaptainAdminNoteDto
            {
                Id = Guid.NewGuid(),
                NoteType = "general",
                Note = c.AdminNote,
                CreatedBy = "Admin",
                CreatedAt = c.UpdatedAt ?? c.CreatedAt
            });
        }

        var requiredDocCount = (detail.ApplicationType == "company" ? 4 : 3) + (c.Boats.Count * 2);

        if (storedFiles.Count < requiredDocCount)
            detail.DocumentSummaryStatus = "missing";
        else if (storedFiles.Any(f => f.Status == "rejected"))
            detail.DocumentSummaryStatus = "rejected";
        else if (storedFiles.Any(f => f.Status == "pendingReview" || string.IsNullOrEmpty(f.Status) || f.Status == "uploaded"))
            detail.DocumentSummaryStatus = "pendingReview";
        else if (storedFiles.Any(f => f.Status == "expiringSoon"))
            detail.DocumentSummaryStatus = "expiringSoon";
        else
            detail.DocumentSummaryStatus = "completed";

        return Result<AdminCaptainDetailDto>.Success(detail);
    }

    public async Task<Result> SuspendCaptainAsync(Guid id, string reason, CancellationToken cancellationToken = default)
    {
        var c = await _dbContext.Captains.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (c == null)
            return Result.Failure("CAPTAIN_NOT_FOUND", "Kaptan bulunamadı.");

        c.AccountStatus = "Suspended";
        c.AdminNote = string.IsNullOrEmpty(c.AdminNote) ? reason : $"{c.AdminNote}\nSuspension Reason: {reason}";
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ReactivateCaptainAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var c = await _dbContext.Captains.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (c == null)
            return Result.Failure("CAPTAIN_NOT_FOUND", "Kaptan bulunamadı.");

        c.AccountStatus = "Active";
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private string? MaskString(string? input, int start, int end)
    {
        if (string.IsNullOrEmpty(input) || input.Length <= start + end)
            return input;

        return $"{input.Substring(0, start)}***{input.Substring(input.Length - end)}";
    }

    public async Task<Result> ApproveDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var doc = await _dbContext.StoredFiles.FirstOrDefaultAsync(f => f.Id == documentId, cancellationToken);
        if (doc == null)
            return Result.Failure("DOCUMENT_NOT_FOUND", "Belge bulunamadı.");

        doc.Status = "approved";

        var audit = new EIskele.Domain.Entities.AuditLog
        {
            Action = "ApproveDocument",
            EntityType = "CaptainDocument",
            EntityId = $"{doc.OwnerUserId}_{doc.FileType}",
            Description = "Belge onaylandı."
        };
        _dbContext.AuditLogs.Add(audit);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> RejectDocumentAsync(Guid documentId, string reason, CancellationToken cancellationToken = default)
    {
        var doc = await _dbContext.StoredFiles.FirstOrDefaultAsync(f => f.Id == documentId, cancellationToken);
        if (doc == null)
            return Result.Failure("DOCUMENT_NOT_FOUND", "Belge bulunamadı.");

        doc.Status = "rejected";
        doc.IsDeleted = true;
        doc.DeletedAt = DateTime.UtcNow;

        var audit = new EIskele.Domain.Entities.AuditLog
        {
            Action = "RejectDocument",
            EntityType = "CaptainDocument",
            EntityId = $"{doc.OwnerUserId}_{doc.FileType}",
            Description = reason
        };
        _dbContext.AuditLogs.Add(audit);
        
        // Physically delete file
        if (!string.IsNullOrEmpty(doc.StoragePath))
        {
            await _fileStorageService.DeleteAsync(doc.StoragePath, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private (string title, string type) GetDocumentInfo(string fileType)
    {
        if (string.IsNullOrEmpty(fileType)) return ("Bilinmeyen Belge", "other");

        if (fileType == "doc_captain_license") return ("Kaptan Yeterlilik Belgesi", "license");
        if (fileType == "doc_captain_id") return ("Kimlik Fotokopisi", "identity");
        if (fileType == "doc_captain_iban") return ("IBAN Doğrulama Belgesi", "other");
        if (fileType == "doc_company_tax") return ("Vergi Levhası", "tax");
        if (fileType.StartsWith("doc_boat_reg_")) return ("Tekne Ruhsatı", "other");
        if (fileType.StartsWith("doc_boat_ins_")) return ("Sigorta Belgesi", "insurance");

        return ("Diğer Belge", "other");
    }
}
