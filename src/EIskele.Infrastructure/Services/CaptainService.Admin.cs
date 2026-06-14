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
            q = q.Where(c => c.Status.Equals(query.ApplicationStatus, StringComparison.OrdinalIgnoreCase));
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
                DocumentStatus = "completed", // Mocked for now, until document workflow is fully built
                ApplicationStatus = c.Status.ToLower(),
                AccountStatus = c.AccountStatus.ToLower(),
                AverageRating = 0, // Mocked for now
                TotalReservationCount = 0, // Mocked for now
                CompletedReservationCount = 0, // Mocked for now
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(cancellationToken);

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
            .Where(f => f.RelatedEntityType == "CaptainDocument" && f.RelatedEntityId == id.ToString())
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
            ApplicationStatus = c.Status.ToLower(),
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

            Documents = storedFiles.Select(f => new CaptainDocumentDto
            {
                Id = f.Id,
                DocumentType = "identity", // Should be parsed from file metadata
                Title = f.OriginalFileName,
                FileName = f.OriginalFileName,
                FileSizeText = $"{Math.Round(f.SizeInBytes / 1024.0, 2)} KB",
                UploadedAt = f.CreatedAt,
                Status = "completed"
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
}
