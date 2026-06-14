using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Boats;
using EIskele.Application.Common.Results;
using EIskele.Domain.Entities;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace EIskele.Infrastructure.Services;

public partial class BoatService : IBoatService
{
    private readonly EIskeleDbContext _dbContext;

    public BoatService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<BoatResponse>> CreateBoatAsync(CreateBoatRequest request, CancellationToken cancellationToken = default)
    {
        var boat = new Boat
        {
            Id = Guid.NewGuid(),
            CaptainId = request.CaptainId,
            LocationId = request.LocationId,
            Name = request.Name,
            Capacity = request.Capacity,
            Status = BoatStatus.Draft
        };

        _dbContext.Boats.Add(boat);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<BoatResponse>.Success(new BoatResponse
        {
            Id = boat.Id,
            Status = boat.Status.ToString()
        });
    }

    public async Task<Result> SubmitForReviewAsync(Guid boatId, CancellationToken cancellationToken = default)
    {
        var boat = await _dbContext.Boats.FindAsync(new object[] { boatId }, cancellationToken);
        if (boat == null)
        {
            return Result.Failure("NOT_FOUND", "Tekne bulunamadı.");
        }

        if (boat.Status != BoatStatus.Draft && boat.Status != BoatStatus.Rejected)
        {
            return Result.Failure("BOAT.INVALID_STATUS", "Sadece taslak veya reddedilmiş tekneler incelemeye gönderilebilir.");
        }

        boat.Status = BoatStatus.UnderReview;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ApproveBoatAsync(Guid boatId, CancellationToken cancellationToken = default)
    {
        var boat = await _dbContext.Boats.FindAsync(new object[] { boatId }, cancellationToken);
        if (boat == null)
        {
            return Result.Failure("NOT_FOUND", "Tekne bulunamadı.");
        }

        if (boat.Status != BoatStatus.UnderReview)
        {
            return Result.Failure("BOAT.NOT_UNDER_REVIEW", "Tekne inceleme aşamasında değil.");
        }

        boat.Status = BoatStatus.Published;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<TourPackageResponse>> AddTourPackageAsync(CreateTourPackageRequest request, CancellationToken cancellationToken = default)
    {
        var boat = await _dbContext.Boats.FindAsync(new object[] { request.BoatId }, cancellationToken);
        if (boat == null)
        {
            return Result<TourPackageResponse>.Failure("NOT_FOUND", "Tekne bulunamadı.");
        }

        if (request.Price <= 0)
        {
            return Result<TourPackageResponse>.Failure("PACKAGE.INVALID_PRICE", "Paket fiyatı sıfır veya negatif olamaz.");
        }

        if (request.MinCapacity > request.MaxCapacity)
        {
            return Result<TourPackageResponse>.Failure("PACKAGE.INVALID_CAPACITY", "Minimum kapasite, maksimum kapasiteden büyük olamaz.");
        }

        if (request.MaxCapacity > boat.Capacity)
        {
            return Result<TourPackageResponse>.Failure("PACKAGE.CAPACITY_EXCEEDED", "Paket kapasitesi tekne kapasitesini aşamaz.");
        }

        var package = new TourPackage
        {
            Id = Guid.NewGuid(),
            BoatId = request.BoatId,
            Name = request.Name,
            Price = request.Price,
            MinCapacity = request.MinCapacity,
            MaxCapacity = request.MaxCapacity,
            IsActive = true,
            ApprovalType = ReservationApprovalType.AutoApprove
        };

        _dbContext.TourPackages.Add(package);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<TourPackageResponse>.Success(new TourPackageResponse
        {
            Id = package.Id
        });
    }

    public async Task<Result<PagedResult<AdminBoatListItemDto>>> GetAdminBoatsAsync(
        string? search, string? boatStatus, string? documentStatus, string? publishStatus,
        string? captainStatus, Guid? locationId, string? boatType,
        int? minCapacity, int? maxCapacity, int page, int pageSize,
        string? sortBy, string? sortDirection, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Boats
            .Include(b => b.Captain)
                .ThenInclude(c => c.User)
            .Include(b => b.Location)
            .Include(b => b.Harbor)
            .Include(b => b.TourPackages)
            .AsQueryable();

        // Filters
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(b => b.Name.Contains(search) || b.Slug.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(boatStatus) && boatStatus != "all")
        {
            if (Enum.TryParse<BoatStatus>(boatStatus, true, out var statusEnum))
            {
                query = query.Where(b => b.Status == statusEnum);
            }
        }

        if (locationId.HasValue)
        {
            query = query.Where(b => b.LocationId == locationId.Value);
        }

        if (minCapacity.HasValue)
        {
            query = query.Where(b => b.Capacity >= minCapacity.Value);
        }

        if (maxCapacity.HasValue)
        {
            query = query.Where(b => b.Capacity <= maxCapacity.Value);
        }

        // Sorting
        query = sortDirection?.ToLower() == "asc" 
            ? query.OrderBy(b => b.CreatedAt) 
            : query.OrderByDescending(b => b.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var boats = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = boats.Select(b => new AdminBoatListItemDto
        {
            Id = b.Id,
            BoatNo = "TR-" + b.Id.ToString().Substring(0, 6).ToUpper(), // mock boat no
            BoatName = b.Name,
            BoatType = "Motoryat", // Mock for now if missing in entity
            CaptainId = b.CaptainId,
            CaptainName = b.Captain?.User != null ? $"{b.Captain.User.FirstName} {b.Captain.User.LastName}" : "Bilinmiyor",
            Location = b.Location.Name,
            Harbor = b.Harbor?.Name,
            Capacity = b.Capacity,
            TotalPackageCount = b.TourPackages.Count,
            ActivePackageCount = b.TourPackages.Count(p => p.IsActive),
            ReviewStatus = b.Status.ToString(),
            PublishStatus = b.Status == BoatStatus.Published ? "published" : "notPublished",
            DocumentStatus = "completed",
            UpdatedAt = b.UpdatedAt
        }).ToList();

        var pagedResult = new PagedResult<AdminBoatListItemDto>(items, totalCount, page, pageSize);
        return Result<PagedResult<AdminBoatListItemDto>>.Success(pagedResult);
    }

    public async Task<Result<AdminBoatsSummaryDto>> GetAdminBoatsSummaryAsync(CancellationToken cancellationToken = default)
    {
        var total = await _dbContext.Boats.CountAsync(cancellationToken);
        var published = await _dbContext.Boats.CountAsync(b => b.Status == BoatStatus.Published, cancellationToken);
        var pending = await _dbContext.Boats.CountAsync(b => b.Status == BoatStatus.UnderReview, cancellationToken);
        var rejected = await _dbContext.Boats.CountAsync(b => b.Status == BoatStatus.Rejected, cancellationToken);
        var passive = await _dbContext.Boats.CountAsync(b => b.Status == BoatStatus.Passive, cancellationToken);
        var suspended = await _dbContext.Boats.CountAsync(b => b.Status == BoatStatus.Suspended, cancellationToken);

        return Result<AdminBoatsSummaryDto>.Success(new AdminBoatsSummaryDto
        {
            TotalBoats = total,
            PublishedBoats = published,
            PendingReviewBoats = pending,
            RejectedBoats = rejected,
            PassiveBoats = passive,
            SuspendedBoats = suspended,
            MissingDocumentBoats = 0,
            RevisionRequestedBoats = 0
        });
    }
}
