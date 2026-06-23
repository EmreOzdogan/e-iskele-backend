using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Captains;
using EIskele.Application.Common.Files;
using EIskele.Application.Common.Results;
using EIskele.Domain.Entities;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Services;

public class CaptainDocumentsService : ICaptainDocumentsService
{
    private readonly EIskeleDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;

    public CaptainDocumentsService(EIskeleDbContext dbContext, IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<CaptainDocumentsDataDto>> GetCaptainDocumentsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var captain = await _dbContext.Captains
            .Include(c => c.Boats)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (captain == null)
        {
            return Result<CaptainDocumentsDataDto>.Failure("CAPTAIN_NOT_FOUND", "Kaptan profili bulunamadı.");
        }

        var expectedDocs = GenerateExpectedDocuments(captain);

        var storedFiles = await _dbContext.Set<StoredFile>()
            .Where(f => f.OwnerUserId == userId && f.RelatedEntityType == "CaptainDocument")
            .ToListAsync(cancellationToken);

        var documents = new List<CaptainHubDocumentDto>();

        var auditLogs = await _dbContext.AuditLogs
            .Where(a => a.EntityType.StartsWith("CaptainDocument_"))
            .ToListAsync(cancellationToken);

        foreach (var expectedDoc in expectedDocs)
        {
            var matchedFile = storedFiles.OrderByDescending(x => x.CreatedAt)
                                         .FirstOrDefault(f => f.FileType.ToString() == expectedDoc.Id);

            var fileLogs = auditLogs.Where(a => a.EntityType == $"CaptainDocument_{expectedDoc.Id}" && a.EntityId == userId.ToString()).OrderBy(a => a.CreatedAt).ToList();
            var lastReject = fileLogs.LastOrDefault(a => a.Action == "RejectDocument");

            if (matchedFile != null)
            {
                expectedDoc.Status = char.ToLowerInvariant(matchedFile.Status.ToString()[0]) + matchedFile.Status.ToString().Substring(1); 
                expectedDoc.FileName = matchedFile.OriginalFileName;
                expectedDoc.UploadedAtText = matchedFile.CreatedAt.ToString("dd MMMM yyyy");

                if (lastReject != null && expectedDoc.Status == "rejected")
                {
                    expectedDoc.RejectionReason = lastReject.Description;
                }
            }
            else
            {
                // File deleted or not uploaded
                expectedDoc.Status = "notUploaded";
                if (lastReject != null)
                {
                    // If we have a reject log but no file, it's missing but we can still show rejection reason as to why it's missing.
                    // Wait, the UI expects status to be "rejected" to show it as "Eksik" (which we mapped from "rejected").
                    // Actually, if it's missing because it was rejected, we can set status to "rejected" so UI badge says "Eksik" and red.
                    expectedDoc.Status = "rejected";
                    expectedDoc.RejectionReason = lastReject.Description;
                }
            }

            foreach (var log in fileLogs)
            {
                if (log.Action == "UploadDocument")
                {
                    expectedDoc.History.Add(new CaptainDocumentHistoryDto
                    {
                        Id = log.Id.ToString(),
                        DateText = log.CreatedAt.ToString("dd MMMM yyyy HH:mm"),
                        Title = "Belge Yüklendi",
                        Description = "Belge sisteme yüklendi."
                    });
                }
                else if (log.Action == "RejectDocument")
                {
                    expectedDoc.History.Add(new CaptainDocumentHistoryDto
                    {
                        Id = log.Id.ToString(),
                        DateText = log.CreatedAt.ToString("dd MMMM yyyy HH:mm"),
                        Title = "Belge Reddedildi",
                        Description = log.Description
                    });
                }
                else if (log.Action == "ApproveDocument")
                {
                    expectedDoc.History.Add(new CaptainDocumentHistoryDto
                    {
                        Id = log.Id.ToString(),
                        DateText = log.CreatedAt.ToString("dd MMMM yyyy HH:mm"),
                        Title = "Belge Onaylandı",
                        Description = "Belge admin tarafından onaylandı."
                    });
                }
            }
            
            // Fallback for upload history if there is a file but no upload log
            if (matchedFile != null && !fileLogs.Any(l => l.Action == "UploadDocument"))
            {
                expectedDoc.History.Insert(0, new CaptainDocumentHistoryDto
                {
                    Id = matchedFile.Id.ToString() + "_upload",
                    DateText = matchedFile.CreatedAt.ToString("dd MMMM yyyy HH:mm"),
                    Title = "Belge Yüklendi",
                    Description = "Belge sisteme yüklendi."
                });
            }
            
            documents.Add(expectedDoc);
        }

        // Add any uploaded documents that are not in the "expected" list (e.g., extra documents)
        var extraFiles = storedFiles.Where(f => !expectedDocs.Any(e => e.Id == f.FileType.ToString())).ToList();
        foreach (var extra in extraFiles)
        {
            documents.Add(new CaptainHubDocumentDto
            {
                Id = extra.FileType.ToString(), // e.g., "extra_doc_1"
                Name = extra.OriginalFileName,
                Category = "extra",
                RequirementLevel = "optional",
                Status = char.ToLowerInvariant(extra.Status.ToString()[0]) + extra.Status.ToString().Substring(1),
                FileName = extra.OriginalFileName,
                UploadedAtText = extra.CreatedAt.ToString("dd MMMM yyyy")
            });
        }

        var summary = new CaptainDocumentsSummaryDto
        {
            Total = documents.Count,
            Approved = documents.Count(d => d.Status == "approved"),
            PendingReview = documents.Count(d => d.Status == "pendingReview"),
            NotUploaded = documents.Count(d => d.Status == "notUploaded"),
            NeedsUpdate = documents.Count(d => d.Status == "needsUpdate"),
            Rejected = documents.Count(d => d.Status == "rejected"),
            ExpiringSoon = documents.Count(d => d.Status == "expiringSoon"),
        };
        
        // simple percentage calculation
        if (summary.Total > 0)
        {
            int completed = summary.Approved + summary.PendingReview; // Or just Approved
            summary.CompletionRate = (int)((double)completed / summary.Total * 100);
        }

        return Result<CaptainDocumentsDataDto>.Success(new CaptainDocumentsDataDto
        {
            Summary = summary,
            Documents = documents
        });
    }

    public async Task<Result> UploadDocumentAsync(Guid userId, string documentId, Stream fileStream, string fileName, string contentType, UploadCaptainDocumentRequest request, CancellationToken cancellationToken = default)
    {
        var captain = await _dbContext.Captains.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        if (captain == null)
            return Result.Failure("CAPTAIN_NOT_FOUND", "Kaptan profili bulunamadı.");

        // documentId examples: "doc_captain_license", "doc_boat_reg_12345678-1234-1234-1234-123456789012"
        var fileRequest = new FileUploadRequest
        {
            Content = fileStream,
            ContentType = contentType,
            OriginalFileName = fileName,
            OwnerUserId = userId,
            RelatedEntityType = "CaptainDocument",
            RelatedEntityId = captain.Id.ToString(),
            FileType = documentId, // using documentId as FileType
            IsPublic = false // Documents are sensitive
        };

        var uploadResult = await _fileStorageService.UploadAsync(fileRequest, cancellationToken);
        if (!uploadResult.Success)
            return Result.Failure("UPLOAD_FAILED", "Dosya yüklenemedi.");

        var newFile = await _dbContext.Set<StoredFile>()
            .FirstOrDefaultAsync(f => f.StoredFileName == uploadResult.StoredFileName, cancellationToken);

        if (newFile != null)
        {
            newFile.Status = StoredFileStatus.Pending;

            var audit = new EIskele.Domain.Entities.AuditLog
            {
                Action = "UploadDocument",
                EntityType = $"CaptainDocument_{documentId}",
                EntityId = userId.ToString(),
                Description = "Belge sisteme yüklendi."
            };
            _dbContext.AuditLogs.Add(audit);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }

    private List<CaptainHubDocumentDto> GenerateExpectedDocuments(Captain captain)
    {
        var list = new List<CaptainHubDocumentDto>
        {
            new() {
                Id = "doc_captain_license",
                Name = "Kaptan Yeterlilik Belgesi",
                Category = "captain",
                RequirementLevel = "required",
                IsSensitive = true
            },
            new() {
                Id = "doc_captain_id",
                Name = "Kimlik Fotokopisi",
                Category = "captain",
                RequirementLevel = "required",
                IsSensitive = true
            },
            new() {
                Id = "doc_captain_iban",
                Name = "IBAN Doğrulama Belgesi",
                Category = "payment",
                RequirementLevel = "required",
                IsSensitive = true
            }
        };

        if (captain.ApplicationType == EIskele.Domain.Enums.CaptainApplicationType.Company)
        {
            list.Add(new CaptainHubDocumentDto
            {
                Id = "doc_company_tax",
                Name = "Vergi Levhası",
                Category = "company",
                RequirementLevel = "requiredForCompany",
                IsSensitive = true
            });
        }

        foreach (var boat in captain.Boats)
        {
            list.Add(new CaptainHubDocumentDto
            {
                Id = $"doc_boat_reg_{boat.Id}",
                Name = "Tekne Ruhsatı",
                Category = "boat",
                RequirementLevel = "requiredForBoat",
                BoatId = boat.Id.ToString(),
                BoatName = boat.Name,
                IsSensitive = true
            });

            list.Add(new CaptainHubDocumentDto
            {
                Id = $"doc_boat_ins_{boat.Id}",
                Name = "Sigorta Belgesi",
                Category = "boat",
                RequirementLevel = "requiredForBoat",
                BoatId = boat.Id.ToString(),
                BoatName = boat.Name,
                IsSensitive = true
            });
        }

        return list;
    }
}
