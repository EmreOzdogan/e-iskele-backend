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
            BoatType = "Motoryat", // Placeholder as it's not in entity
            BrandModel = "Bilinmiyor",
            ProductionYear = null,
            Length = null,
            Capacity = boat.Capacity,
            LicenseNo = "RH-1234-5678", // Placeholder
            Description = "Açıklama",
            
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
            PublishStatus = boat.Status == BoatStatus.Published ? "Yayında" : "Yayında Değil",
            ReviewStatus = boat.Status == BoatStatus.UnderReview ? "İncelemede" : "Kontrol Edildi",
            DocumentStatus = "Kontrol Bekliyor", // Placeholder

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
            Status = f.Status.ToString(),
            FileName = f.OriginalFileName,
            UploadedAt = f.CreatedAt
        }).ToList();

        // Add mock image if empty for testing Admin UI
        if (!dtos.Any())
        {
            dtos.Add(new BoatImageDto { Id = Guid.NewGuid(), ImageUrl = "https://images.unsplash.com/photo-1567899378494-47b22a2ae96a", ImageType = "cover", Status = "pending", FileName = "mock-cover.jpg", UploadedAt = DateTime.UtcNow });
        }

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
            DocumentType = f.FileType.ToString(),
            DocumentName = f.FileType == StoredFileType.BoatLicenseDocument.ToString() ? "Tekne Ruhsatı" : "Sigorta Belgesi",
            FileName = f.OriginalFileName,
            FileSize = (f.SizeInBytes / 1024) + " KB",
            Status = f.Status.ToString(),
            UploadedAt = f.CreatedAt,
            ValidUntil = null,
            DownloadUrl = f.PublicUrl
        }).ToList();
        
        // Add mock doc if empty
        if (!dtos.Any())
        {
            dtos.Add(new BoatDocumentDto { Id = Guid.NewGuid(), DocumentType = "BoatLicense", DocumentName = "Tekne Ruhsatı", FileName = "ruhsat.pdf", FileSize = "1.2 MB", Status = "pending", UploadedAt = DateTime.UtcNow, DownloadUrl = "#" });
        }

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

        // Add mock features if empty
        if (!dtos.Any())
        {
            dtos.Add(new BoatFeatureDto { Id = Guid.NewGuid(), Name = "Can yeleği", Category = "safety", IsAvailable = true, Status = "Kontrol Bekliyor" });
            dtos.Add(new BoatFeatureDto { Id = Guid.NewGuid(), Name = "WC", Category = "feature", IsAvailable = true, Status = "Uygun" });
        }

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
            TourType = "Bilinmiyor",
            DurationHours = 0,
            MinCapacity = p.MinCapacity,
            MaxCapacity = p.MaxCapacity,
            Price = p.Price,
            ApprovalType = "Kaptan Onaylı",
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
