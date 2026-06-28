using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EIskele.Application.Boats;
using EIskele.Application.Common.Results;
using EIskele.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Services;

public partial class BoatService
{
    public async Task<Result<BoatDetailDto>> GetAdminBoatDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var boat = await _dbContext.Boats
            .Include(b => b.Captain)
                .ThenInclude(c => c.User)
            .Include(b => b.Location)
            .Include(b => b.Harbor)
            .Include(b => b.TourPackages)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        if (boat == null)
        {
            return Result<BoatDetailDto>.Failure("Tekne bulunamadı.", "NOT_FOUND");
        }

        var dto = new BoatDetailDto
        {
            Id = boat.Id,
            BoatNo = $"BOAT-{boat.CreatedAt.Year}-{boat.Id.ToString().Substring(0, 4).ToUpper()}",
            BoatName = boat.Name,
            BoatType = string.IsNullOrWhiteSpace(boat.BoatType) ? "Bilinmiyor" : boat.BoatType,
            BrandModel = string.IsNullOrWhiteSpace(boat.BrandModel) ? "Bilinmiyor" : boat.BrandModel,
            ProductionYear = int.TryParse(boat.ProductionYear, out var pYear) ? pYear : null,
            Length = decimal.TryParse(boat.Length, out var bLen) ? bLen : null,
            Capacity = boat.Capacity,
            LicenseNo = "Bilinmiyor", // License no will be retrieved from documents later if needed
            Description = boat.Description,
            
            CaptainId = boat.CaptainId,
            CaptainName = boat.Captain.User.FirstName + " " + boat.Captain.User.LastName,
            CaptainApplicationType = "Bireysel",
            CaptainStatus = "Onaylı",
            CaptainPhone = boat.Captain.User.PhoneNumber ?? "",
            CaptainEmail = boat.Captain.User.Email ?? "",

            Location = boat.Location.Name,
            Harbor = boat.Harbor?.Name,

            ActivePackageCount = boat.TourPackages.Count(p => p.IsActive),
            TotalPackageCount = boat.TourPackages.Count,

            BoatStatus = boat.Status.ToString(),
            PublishStatus = boat.Status switch {
                BoatStatus.Published => "published",
                BoatStatus.Passive => "passive",
                BoatStatus.Suspended => "suspended",
                _ => "notPublished"
            },
            ReviewStatus = boat.Status switch {
                BoatStatus.Draft => "draft",
                BoatStatus.UnderReview => "inReview",
                BoatStatus.Published => "approved",
                BoatStatus.Passive => "approved",
                BoatStatus.Rejected => "rejected",
                BoatStatus.Suspended => "suspended",
                _ => "inReview"
            },
            DocumentStatus = "completed",

            UpdatedAt = boat.UpdatedAt,
            SubmittedAt = boat.CreatedAt
        };

        return Result<BoatDetailDto>.Success(dto);
    }

    public async Task<Result<List<BoatImageDto>>> GetAdminBoatImagesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var images = await _dbContext.StoredFiles
            .Where(f => f.RelatedEntityType == "Boat" && f.RelatedEntityId == id.ToString() && (f.FileType == StoredFileType.BoatCoverImage.ToString() || f.FileType == StoredFileType.BoatImage.ToString()))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var dtos = images.Select(f => new BoatImageDto
        {
            Id = f.Id,
            ImageUrl = f.PublicUrl,
            ImageType = f.FileType == StoredFileType.BoatCoverImage.ToString() ? "cover" : "gallery",
            Status = f.Status == EIskele.Domain.Enums.StoredFileStatus.Pending ? "pendingReview" : f.Status == EIskele.Domain.Enums.StoredFileStatus.Approved ? "approved" : "rejected",
            FileName = f.OriginalFileName,
            UploadedAt = f.CreatedAt
        }).ToList();

        return Result<List<BoatImageDto>>.Success(dtos);
    }

    public async Task<Result<List<BoatDocumentDto>>> GetAdminBoatDocumentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var documents = await _dbContext.StoredFiles
            .Where(f => f.RelatedEntityType == "Boat" && f.RelatedEntityId == id.ToString() && (f.FileType == StoredFileType.BoatLicenseDocument.ToString() || f.FileType == StoredFileType.InsuranceDocument.ToString()))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var dtos = documents.Select(f => new BoatDocumentDto
        {
            Id = f.Id,
            DocumentType = f.FileType == EIskele.Domain.Enums.StoredFileType.BoatLicenseDocument.ToString() ? "boatLicense" : "insurance",
            DocumentName = f.FileType == StoredFileType.BoatLicenseDocument.ToString() ? "Tekne Ruhsatı" : "Sigorta Belgesi",
            FileName = f.OriginalFileName,
            FileSize = (f.SizeInBytes / 1024) + " KB",
            Status = f.Status == EIskele.Domain.Enums.StoredFileStatus.Pending ? "pendingReview" : f.Status == EIskele.Domain.Enums.StoredFileStatus.Approved ? "approved" : "rejected",
            UploadedAt = f.CreatedAt,
            ValidUntil = f.ValidUntil,
            DownloadUrl = f.PublicUrl
        }).ToList();
        
        return Result<List<BoatDocumentDto>>.Success(dtos);
    }

    public async Task<Result<List<BoatFeatureDto>>> GetAdminBoatFeaturesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var features = await _dbContext.BoatFeatures
            .Where(f => f.BoatId == id)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var dtos = features.Select(f => new BoatFeatureDto
        {
            Id = f.Id,
            Name = f.Name,
            Category = f.Category,
            IsAvailable = f.IsAvailable,
            Status = f.Status.ToString()
        }).ToList();

        return Result<List<BoatFeatureDto>>.Success(dtos);
    }

    public async Task<Result<List<BoatPackageSummaryDto>>> GetAdminBoatPackagesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var packages = await _dbContext.TourPackages
            .Where(p => p.BoatId == id)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var dtos = packages.Select(p => new BoatPackageSummaryDto
        {
            Id = p.Id,
            PackageName = p.Name,
            TourType = string.IsNullOrWhiteSpace(p.TourType) ? "Bilinmiyor" : p.TourType,
            DurationHours = p.DurationHours,
            MinCapacity = p.MinCapacity,
            MaxCapacity = p.MaxCapacity,
            Price = p.Price,
            ApprovalType = p.ApprovalType == ReservationApprovalType.AutoApprove ? "Otomatik Onay" : "Kaptan Onaylı",
            Status = p.IsActive ? "Aktif" : "Pasif"
        }).ToList();

        return Result<List<BoatPackageSummaryDto>>.Success(dtos);
    }

    public async Task<Result> RejectBoatAsync(Guid id, string reason, CancellationToken cancellationToken = default)
    {
        var boat = await _dbContext.Boats.FindAsync(new object[] { id }, cancellationToken);
        if (boat == null) return Result.Failure("NOT_FOUND", "Tekne bulunamadı.");
        
        boat.Status = BoatStatus.Rejected;
        // add reason to UserAdminNote or something
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> RequestBoatRevisionAsync(Guid id, List<string> fields, string note, CancellationToken cancellationToken = default)
    {
        var boat = await _dbContext.Boats.FindAsync(new object[] { id }, cancellationToken);
        if (boat == null) return Result.Failure("NOT_FOUND", "Tekne bulunamadı.");
        
        boat.Status = BoatStatus.Draft; // or NeedsRevision
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeactivateBoatAsync(Guid id, string reason, CancellationToken cancellationToken = default)
    {
        var boat = await _dbContext.Boats.FindAsync(new object[] { id }, cancellationToken);
        if (boat == null) return Result.Failure("NOT_FOUND", "Tekne bulunamadı.");
        
        boat.Status = BoatStatus.Passive;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> SuspendBoatAsync(Guid id, string reason, CancellationToken cancellationToken = default)
    {
        var boat = await _dbContext.Boats.FindAsync(new object[] { id }, cancellationToken);
        if (boat == null) return Result.Failure("NOT_FOUND", "Tekne bulunamadı.");
        
        boat.Status = BoatStatus.Suspended;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ReactivateBoatAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var boat = await _dbContext.Boats.FindAsync(new object[] { id }, cancellationToken);
        if (boat == null) return Result.Failure("NOT_FOUND", "Tekne bulunamadı.");
        
        boat.Status = BoatStatus.Published;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ApproveBoatImageAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        var file = await _dbContext.StoredFiles.FindAsync(new object[] { imageId }, cancellationToken);
        if (file != null)
        {
            file.Status = StoredFileStatus.Approved;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        return Result.Success();
    }

    public async Task<Result> RejectBoatImageAsync(Guid imageId, string reason, CancellationToken cancellationToken = default)
    {
        var file = await _dbContext.StoredFiles.FindAsync(new object[] { imageId }, cancellationToken);
        if (file != null)
        {
            file.Status = StoredFileStatus.Rejected;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        return Result.Success();
    }

    public async Task<Result> ApproveBoatDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var file = await _dbContext.StoredFiles.FindAsync(new object[] { documentId }, cancellationToken);
        if (file != null)
        {
            file.Status = StoredFileStatus.Approved;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        return Result.Success();
    }

    public async Task<Result> RejectBoatDocumentAsync(Guid documentId, string reason, CancellationToken cancellationToken = default)
    {
        var file = await _dbContext.StoredFiles.FindAsync(new object[] { documentId }, cancellationToken);
        if (file != null)
        {
            file.Status = StoredFileStatus.Rejected;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        return Result.Success();
    }
}



