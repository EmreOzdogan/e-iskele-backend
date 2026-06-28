using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Boats;
using EIskele.Application.Common.Results;
using EIskele.Domain.Entities;
using EIskele.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Services;

public partial class BoatService
{
    public async Task<Result<List<CaptainBoatListItemDto>>> GetMyBoatsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var captain = await _dbContext.Captains
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (captain == null)
        {
            return Result<List<CaptainBoatListItemDto>>.Success(new List<CaptainBoatListItemDto>());
        }

        var boats = await _dbContext.Boats
            .Include(b => b.Location)
            .Include(b => b.Harbor)
            .Include(b => b.TourPackages)
            .Include(b => b.Reservations)
            .Where(b => b.CaptainId == captain.Id)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);

        var boatIdsStr = boats.Select(b => b.Id.ToString()).ToList();
        var boatCoverImageFileType = EIskele.Domain.Enums.StoredFileType.BoatCoverImage.ToString();
        var boatCoverImages = await _dbContext.StoredFiles
            .Where(f => f.RelatedEntityType == "Boat" && boatIdsStr.Contains(f.RelatedEntityId) && f.FileType == boatCoverImageFileType)
            .ToListAsync(cancellationToken);

        var dtos = boats.Select(b => 
        {
            var coverImage = boatCoverImages.FirstOrDefault(f => f.RelatedEntityId == b.Id.ToString());
            return new CaptainBoatListItemDto
            {
                Id = b.Id,
                Name = b.Name,
                BoatType = b.BoatType,
                Status = b.Status.ToString(),
                Capacity = b.Capacity,
                LocationName = b.Location?.Name ?? string.Empty,
                HarborName = b.Harbor?.Name ?? string.Empty,
                CoverImageUrl = coverImage?.PublicUrl,
                ActivePackageCount = b.TourPackages.Count(p => p.IsActive),
                UpcomingReservationCount = b.Reservations.Count(r => r.Status == ReservationStatus.Approved || r.Status == ReservationStatus.Paid),
                CreatedAt = b.CreatedAt
            };
        }).ToList();

        return Result<List<CaptainBoatListItemDto>>.Success(dtos);
    }

    public async Task<Result<CaptainBoatDetailDto>> GetMyBoatDetailAsync(Guid boatId, Guid userId, CancellationToken cancellationToken = default)
    {
        var captain = await _dbContext.Captains
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (captain == null)
        {
            return Result<CaptainBoatDetailDto>.Failure("CAPTAIN_NOT_FOUND", "Kaptan profili bulunamadı.");
        }

        var boat = await _dbContext.Boats
            .Include(b => b.Location)
            .Include(b => b.Harbor)
            .Include(b => b.BoatFeatures)
            .FirstOrDefaultAsync(b => b.Id == boatId && b.CaptainId == captain.Id, cancellationToken);

        if (boat == null)
        {
            return Result<CaptainBoatDetailDto>.Failure("BOAT_NOT_FOUND", "Tekne bulunamadı veya bu tekneye erişim yetkiniz yok.");
        }

        var files = await _dbContext.StoredFiles
            .Where(f => f.RelatedEntityType == "Boat" && f.RelatedEntityId == boat.Id.ToString())
            .ToListAsync(cancellationToken);

        var dto = new CaptainBoatDetailDto
        {
            Id = boat.Id,
            Name = boat.Name,
            Status = boat.Status.ToString(),
            Capacity = boat.Capacity,
            LocationId = boat.LocationId,
            HarborId = boat.HarborId,
            Description = boat.Description,
            BrandModel = boat.BrandModel,
            BoatType = boat.BoatType,
            ProductionYear = boat.ProductionYear,
            Length = boat.Length,
            Features = boat.BoatFeatures.Where(f => f.IsAvailable).Select(f => f.Name).ToList(),
            Images = files.Where(f => f.FileType == "BoatImage" || f.FileType == "BoatCoverImage").Select(f => new StoredFileDto
            {
                Id = f.Id,
                OriginalFileName = f.OriginalFileName,
                FileType = f.FileType,
                PublicUrl = $"/api/files/{f.Id}/download",
                CreatedAt = f.CreatedAt
            }).OrderBy(f => f.CreatedAt).ToList(),
            Documents = files.Where(f => f.FileType == "BoatLicenseDocument" || f.FileType == "InsuranceDocument").Select(f => new StoredFileDto
            {
                Id = f.Id,
                OriginalFileName = f.OriginalFileName,
                FileType = f.FileType,
                PublicUrl = $"/api/files/{f.Id}/download",
                CreatedAt = f.CreatedAt
            }).OrderBy(f => f.CreatedAt).ToList()
        };

        return Result<CaptainBoatDetailDto>.Success(dto);
    }

    public async Task<Result> UpdateMyBoatAsync(Guid boatId, Guid userId, UpdateCaptainBoatRequest request, CancellationToken cancellationToken = default)
    {
        var captain = await _dbContext.Captains
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (captain == null)
        {
            return Result.Failure("CAPTAIN_NOT_FOUND", "Kaptan profili bulunamadı.");
        }

        var boat = await _dbContext.Boats
            .Include(b => b.BoatFeatures)
            .FirstOrDefaultAsync(b => b.Id == boatId && b.CaptainId == captain.Id, cancellationToken);

        if (boat == null)
        {
            return Result.Failure("BOAT_NOT_FOUND", "Tekne bulunamadı veya bu tekneye erişim yetkiniz yok.");
        }

        // Update basic info
        boat.Name = request.Name;
        boat.Capacity = request.Capacity;
        boat.LocationId = request.LocationId;
        boat.HarborId = request.HarborId;
        boat.Description = request.Description;
        boat.BrandModel = request.BrandModel;
        boat.BoatType = request.BoatType;
        boat.ProductionYear = request.ProductionYear;
        boat.Length = request.Length;

        // If published, push back to under review
        if (boat.Status == BoatStatus.Published)
        {
            boat.Status = BoatStatus.UnderReview;
        }

        // Update features (Overwrite strategy)
        _dbContext.BoatFeatures.RemoveRange(boat.BoatFeatures);
        
        foreach (var featureName in request.Features)
        {
            _dbContext.BoatFeatures.Add(new BoatFeature
            {
                Id = Guid.NewGuid(),
                BoatId = boat.Id,
                Name = featureName,
                Category = "Genel",
                IsAvailable = true,
                Status = BoatFeatureStatus.PendingReview
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<BoatResponse>> CreateMyBoatAsync(Guid userId, CreateCaptainBoatRequest request, CancellationToken cancellationToken = default)
    {
        var captain = await _dbContext.Captains
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (captain == null)
        {
            return Result<BoatResponse>.Failure("CAPTAIN_NOT_FOUND", "Kaptan profili bulunamadı.");
        }

        var boat = new Boat
        {
            Id = Guid.NewGuid(),
            CaptainId = captain.Id,
            LocationId = request.LocationId,
            HarborId = request.HarborId,
            Name = request.Name,
            Capacity = request.Capacity,
            Description = request.Description,
            BrandModel = request.BrandModel,
            BoatType = request.BoatType,
            ProductionYear = request.ProductionYear,
            Length = request.Length,
            Status = BoatStatus.Draft,
            Slug = Guid.NewGuid().ToString("N").Substring(0, 8) // Temp slug
        };

        foreach (var featureName in request.Features)
        {
            boat.BoatFeatures.Add(new BoatFeature
            {
                Id = Guid.NewGuid(),
                BoatId = boat.Id,
                Name = featureName,
                Category = "Genel",
                IsAvailable = true,
                Status = BoatFeatureStatus.PendingReview
            });
        }

        _dbContext.Boats.Add(boat);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<BoatResponse>.Success(new BoatResponse
        {
            Id = boat.Id,
            Status = boat.Status.ToString()
        });
    }

    public async Task<Result> DeactivateMyBoatAsync(Guid boatId, Guid userId, CancellationToken cancellationToken = default)
    {
        var captain = await _dbContext.Captains
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (captain == null)
        {
            return Result.Failure("CAPTAIN_NOT_FOUND", "Kaptan profili bulunamadı.");
        }

        var boat = await _dbContext.Boats
            .FirstOrDefaultAsync(b => b.Id == boatId && b.CaptainId == captain.Id, cancellationToken);

        if (boat == null)
        {
            return Result.Failure("BOAT_NOT_FOUND", "Tekne bulunamadı veya bu tekneye erişim yetkiniz yok.");
        }

        if (boat.Status == BoatStatus.Draft || boat.Status == BoatStatus.Rejected || boat.Status == BoatStatus.Suspended)
        {
            return Result.Failure("INVALID_STATUS", "Bu durumdaki tekne pasife alınamaz.");
        }

        boat.Status = BoatStatus.Passive;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteMyBoatAsync(Guid boatId, Guid userId, CancellationToken cancellationToken = default)
    {
        var captain = await _dbContext.Captains
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (captain == null)
        {
            return Result.Failure("CAPTAIN_NOT_FOUND", "Kaptan profili bulunamadı.");
        }

        var boat = await _dbContext.Boats
            .FirstOrDefaultAsync(b => b.Id == boatId && b.CaptainId == captain.Id, cancellationToken);

        if (boat == null)
        {
            return Result.Failure("BOAT_NOT_FOUND", "Tekne bulunamadı veya bu tekneye erişim yetkiniz yok.");
        }

        // Soft delete logic
        boat.IsDeleted = true;
        boat.DeletedAt = DateTime.UtcNow;
        boat.DeletedBy = userId;

        // Also deactivate packages associated with this boat? Not strictly required if global query filter covers relations,
        // but it is safer to also soft delete them or at least set them to inactive, but for now just deleting the boat is fine.
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
