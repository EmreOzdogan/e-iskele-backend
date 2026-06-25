using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Captains;
using EIskele.Application.Common.Results;
using EIskele.Domain.Entities;
using EIskele.Persistence;
using EIskele.Domain.Enums;
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
            ApprovedCaptains = captains.Count(c => c.Status == CaptainStatus.Approved),
            PendingApplications = captains.Count(c => c.Status == CaptainStatus.UnderReview),
            MissingDocuments = captains.Count(c => c.Status == CaptainStatus.MissingDocument),
            SuspendedCaptains = captains.Count(c => c.AccountStatus == CaptainAccountStatus.Suspended),
            CompanyApplications = captains.Count(c => c.ApplicationType == CaptainApplicationType.Company),
            IndividualApplications = captains.Count(c => c.ApplicationType == CaptainApplicationType.Individual),
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
                q = q.Where(c => c.Status == CaptainStatus.UnderReview);
            else if (appStatus == "missingdocument")
                q = q.Where(c => c.Status == CaptainStatus.MissingDocument);
            else if (appStatus == "suspended")
                q = q.Where(c => c.AccountStatus == CaptainAccountStatus.Suspended);
            else
                q = q.Where(c => c.Status.ToString().ToLower() == appStatus);
        }

        if (!string.IsNullOrWhiteSpace(query.CaptainStatus))
        {
            q = q.Where(c => c.AccountStatus.ToString().Equals(query.CaptainStatus, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.ApplicationType))
        {
            q = q.Where(c => c.ApplicationType.ToString().Equals(query.ApplicationType, StringComparison.OrdinalIgnoreCase));
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
                ApplicationType = c.ApplicationType.ToString().ToLower(),
                Email = c.User.Email ?? string.Empty,
                Phone = c.User.PhoneNumber ?? string.Empty,
                Location = c.Location,
                Harbor = c.Harbor,
                TotalBoatCount = c.Boats.Count,
                ActiveBoatCount = c.Boats.Count(b => b.Status == EIskele.Domain.Enums.BoatStatus.Published),
                DocumentStatus = "", // Will be populated dynamically below
                ApplicationStatus = c.Status == CaptainStatus.UnderReview ? "inReview" : c.Status == CaptainStatus.MissingDocument ? "missingDocument" : c.Status.ToString().ToLower(),
                AccountStatus = c.AccountStatus.ToString().ToLower(),
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
            else if (captainFiles.Any(f => f.Status == StoredFileStatus.Rejected))
                item.DocumentStatus = "rejected";
            else if (captainFiles.Any(f => f.Status == StoredFileStatus.Pending))
                item.DocumentStatus = "pendingReview";
            else
                item.DocumentStatus = "completed";
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

        var auditLogs = await _dbContext.AuditLogs
            .Where(a => a.ActorUserId == c.UserId || a.Description.Contains(c.Id.ToString()))
            .OrderByDescending(a => a.CreatedAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        var storedFiles = await _dbContext.StoredFiles
            .Where(f => f.RelatedEntityType == "CaptainDocument" && f.RelatedEntityId == id.ToString() && !f.IsDeleted && f.Status != StoredFileStatus.Rejected)
            .ToListAsync(cancellationToken);

        var expectedDocs = GenerateExpectedDocumentsForAdmin(c);

        var documents = new List<CaptainDocumentDto>();

        foreach (var expectedDoc in expectedDocs)
        {
            var f = storedFiles.FirstOrDefault(x => x.FileType.ToString() == expectedDoc.Id);
            if (f != null)
            {
                documents.Add(new CaptainDocumentDto
                {
                    Id = f.Id,
                    DocumentType = expectedDoc.Category,
                    Title = expectedDoc.Name,
                    FileName = f.OriginalFileName,
                    FileSizeText = $"{Math.Round(f.SizeInBytes / 1024.0, 2)} KB",
                    UploadedAt = f.CreatedAt,
                    Status = f.Status == StoredFileStatus.Approved ? "completed" : char.ToLowerInvariant(f.Status.ToString()[0]) + f.Status.ToString().Substring(1)
                });
            }
            else
            {
                var lastRejectLog = auditLogs.Where(a => a.Action == "RejectDocument" || a.Action == "DeleteDocument")
                                             .FirstOrDefault(a => a.EntityType == $"CaptainDocument_{expectedDoc.Id}");

                documents.Add(new CaptainDocumentDto
                {
                    Id = Guid.Empty, // Placeholder for missing doc
                    DocumentType = expectedDoc.Category,
                    Title = expectedDoc.Name,
                    FileName = null,
                    FileSizeText = null,
                    UploadedAt = null,
                    Status = lastRejectLog != null ? "rejected" : "missing",
                    AdminNote = lastRejectLog?.Description
                });
            }
        }

        var extraFiles = storedFiles.Where(f => !expectedDocs.Any(e => e.Id == f.FileType.ToString())).ToList();
        foreach (var extra in extraFiles)
        {
            documents.Add(new CaptainDocumentDto
            {
                Id = extra.Id,
                DocumentType = "other",
                Title = extra.OriginalFileName,
                FileName = extra.OriginalFileName,
                FileSizeText = $"{Math.Round(extra.SizeInBytes / 1024.0, 2)} KB",
                UploadedAt = extra.CreatedAt,
                Status = extra.Status == StoredFileStatus.Approved ? "completed" : char.ToLowerInvariant(extra.Status.ToString()[0]) + extra.Status.ToString().Substring(1)
            });
        }

        var detail = new AdminCaptainDetailDto
        {
            Id = c.Id,
            ApplicationNo = string.IsNullOrEmpty(c.ApplicationNo) ? $"APP-{c.Id.ToString().Substring(0,6).ToUpper()}" : c.ApplicationNo,
            ApplicationType = c.ApplicationType.ToString().ToLower(),
            DisplayName = $"{c.User.FirstName} {c.User.LastName}",
            Email = c.User.Email ?? string.Empty,
            Phone = c.User.PhoneNumber ?? string.Empty,
            Location = c.Location,
            Harbor = c.Harbor,
            ApplicationStatus = c.Status == CaptainStatus.UnderReview ? "inReview" : c.Status == CaptainStatus.MissingDocument ? "missingDocument" : c.Status.ToString().ToLower(),
            AccountStatus = c.AccountStatus.ToString().ToLower(),
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

            Documents = documents,
            
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
        else if (storedFiles.Any(f => f.Status == StoredFileStatus.Rejected))
            detail.DocumentSummaryStatus = "rejected";
        else if (storedFiles.Any(f => f.Status == StoredFileStatus.Pending))
            detail.DocumentSummaryStatus = "pendingReview";
        else
            detail.DocumentSummaryStatus = "completed";

        return Result<AdminCaptainDetailDto>.Success(detail);
    }

    public async Task<Result> SuspendCaptainAsync(Guid id, string reason, CancellationToken cancellationToken = default)
    {
        var c = await _dbContext.Captains.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (c == null)
            return Result.Failure("CAPTAIN_NOT_FOUND", "Kaptan bulunamadı.");

        c.AccountStatus = CaptainAccountStatus.Suspended;
        c.AdminNote = string.IsNullOrEmpty(c.AdminNote) ? reason : $"{c.AdminNote}\nSuspension Reason: {reason}";
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ReactivateCaptainAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var c = await _dbContext.Captains.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (c == null)
            return Result.Failure("CAPTAIN_NOT_FOUND", "Kaptan bulunamadı.");

        c.AccountStatus = CaptainAccountStatus.Active;
        
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

        doc.Status = StoredFileStatus.Approved;

        var audit = new EIskele.Domain.Entities.AuditLog
        {
            Action = "ApproveDocument",
            EntityType = $"CaptainDocument_{doc.FileType}",
            EntityId = doc.OwnerUserId.ToString(),
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

        var audit = new EIskele.Domain.Entities.AuditLog
        {
            Action = "RejectDocument",
            EntityType = $"CaptainDocument_{doc.FileType}",
            EntityId = doc.OwnerUserId.ToString(),
            Description = reason
        };
        _dbContext.AuditLogs.Add(audit);
        
        // Physically delete file
        if (!string.IsNullOrEmpty(doc.StoragePath))
        {
            await _fileStorageService.DeleteAsync(doc.StoragePath, cancellationToken);
        }

        // Instead of hard deleting, we mark it as rejected so the Captain panel timeline can still show it,
        // but it will be filtered out from Admin lists.
        doc.Status = StoredFileStatus.Rejected;
        doc.StoragePath = string.Empty;
        doc.PublicUrl = string.Empty;
        doc.SizeInBytes = 0;
        doc.OriginalFileName = "SİLİNMİŞ DOSYA";

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var doc = await _dbContext.StoredFiles.FirstOrDefaultAsync(f => f.Id == documentId, cancellationToken);
        if (doc == null)
            return Result.Failure("DOCUMENT_NOT_FOUND", "Belge bulunamadı.");

        var audit = new EIskele.Domain.Entities.AuditLog
        {
            Action = "DeleteDocument",
            EntityType = $"CaptainDocument_{doc.FileType}",
            EntityId = doc.OwnerUserId.ToString(),
            Description = "Admin tarafından silindi."
        };
        _dbContext.AuditLogs.Add(audit);

        // Physically delete file
        if (!string.IsNullOrEmpty(doc.StoragePath))
        {
            await _fileStorageService.DeleteAsync(doc.StoragePath, cancellationToken);
        }

        doc.Status = StoredFileStatus.Rejected;
        doc.StoragePath = string.Empty;
        doc.PublicUrl = string.Empty;
        doc.SizeInBytes = 0;
        doc.OriginalFileName = "SİLİNMİŞ DOSYA";
        doc.IsDeleted = true;
        doc.DeletedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private class AdminExpectedDocDto { public string Id { get; set; } = ""; public string Name { get; set; } = ""; public string Category { get; set; } = ""; }

    private List<AdminExpectedDocDto> GenerateExpectedDocumentsForAdmin(Captain captain)
    {
        var list = new List<AdminExpectedDocDto>
        {
            new() { Id = "doc_captain_license", Name = "Kaptan Yeterlilik Belgesi", Category = "captainLicense" },
            new() { Id = "doc_captain_id", Name = "Kimlik Fotokopisi", Category = "identity" },
            new() { Id = "doc_captain_iban", Name = "IBAN Doğrulama Belgesi", Category = "iban" }
        };

        if (captain.ApplicationType == CaptainApplicationType.Company)
        {
            list.Add(new AdminExpectedDocDto { Id = "doc_company_tax", Name = "Vergi Levhası", Category = "taxCertificate" });
        }

        foreach (var boat in captain.Boats)
        {
            list.Add(new AdminExpectedDocDto { Id = $"doc_boat_reg_{boat.Id}", Name = "Tekne Ruhsatı", Category = "boatLicense" });
            list.Add(new AdminExpectedDocDto { Id = $"doc_boat_ins_{boat.Id}", Name = "Sigorta Belgesi", Category = "insurance" });
        }

        return list;
    }
}
