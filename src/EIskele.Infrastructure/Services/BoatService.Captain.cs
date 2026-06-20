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
            return Result<List<CaptainBoatListItemDto>>.Failure("CAPTAIN_NOT_FOUND", "Kaptan profili bulunamadı.");
        }

        var boats = await _dbContext.Boats
            .Include(b => b.Location)
            .Include(b => b.Harbor)
            .Where(b => b.CaptainId == captain.Id)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);

        var dtos = boats.Select(b => new CaptainBoatListItemDto
        {
            Id = b.Id,
            Name = b.Name,
            Status = b.Status.ToString(),
            Capacity = b.Capacity,
            LocationName = b.Location?.Name ?? string.Empty,
            HarborName = b.Harbor?.Name ?? string.Empty,
            CreatedAt = b.CreatedAt
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

        var dto = new CaptainBoatDetailDto
        {
            Id = boat.Id,
            Name = boat.Name,
            Status = boat.Status.ToString(),
            Capacity = boat.Capacity,
            LocationId = boat.LocationId,
            HarborId = boat.HarborId,
            Description = "", // Assuming missing from entity for MVP or add later
            BrandModel = "", // Assuming missing
            BoatType = "", // Assuming missing
            ProductionYear = "", // Assuming missing
            Length = "", // Assuming missing
            Features = boat.BoatFeatures.Where(f => f.IsAvailable).Select(f => f.Name).ToList()
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
                Status = "Kontrol Bekliyor"
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
                Status = "Kontrol Bekliyor"
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
}
