using EIskele.Application.Common.Locations;
using EIskele.Application.Common.Results;
using EIskele.Domain.Entities;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace EIskele.Infrastructure.Locations;

public class LocationService : ILocationService
{
    private readonly EIskeleDbContext _context;

    public LocationService(EIskeleDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<AdminLocationListItemDto>>> GetAdminLocationsAsync(
        string? search, LocationType? type, LocationStatus? status, bool? isPopular,
        LocationSeoStatus? seoStatus, LocationCoordinateStatus? coordinateStatus,
        LocationRegion? region, int? minBoatCount, int? maxBoatCount,
        int page, int pageSize, string? sortBy, string? sortDirection, CancellationToken cancellationToken)
    {
        var query = _context.Locations
            .Include(l => l.Harbors)
            .Include(l => l.Boats)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Name.Contains(search) || x.Slug.Contains(search));
        }

        if (type.HasValue)
        {
            query = query.Where(x => x.Type == type.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (isPopular.HasValue)
        {
            query = query.Where(x => x.IsPopular == isPopular.Value);
        }

        if (seoStatus.HasValue)
        {
            query = query.Where(x => x.SeoStatus == seoStatus.Value);
        }

        if (coordinateStatus.HasValue)
        {
            query = query.Where(x => x.CoordinateStatus == coordinateStatus.Value);
        }

        if (region.HasValue)
        {
            query = query.Where(x => x.Region == region.Value);
        }

        // Sorting
        query = sortDirection?.ToLower() == "desc" 
            ? query.OrderByDescending(x => x.CreatedAt) 
            : query.OrderBy(x => x.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AdminLocationListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Slug = x.Slug,
                LocationType = x.Type.ToString(),
                Region = x.Region.ToString(),
                CoverImageUrl = x.CoverImageUrl,
                HarborCount = x.Harbors.Count,
                MainHarborName = x.Harbors.Where(h => h.IsMainDeparturePoint).Select(h => h.Name).FirstOrDefault(),
                TotalBoatCount = x.Boats.Count,
                ActiveBoatCount = x.Boats.Count(b => b.Status == BoatStatus.Published),
                TotalPackageCount = 0, // Mock for now
                ActivePackageCount = 0, // Mock for now
                TotalReservationCount = 0, // Mock for now
                MonthlyReservationCount = 0, // Mock for now
                SeoStatus = x.SeoStatus.ToString(),
                CoordinateStatus = x.CoordinateStatus.ToString(),
                Status = x.Status.ToString(),
                IsPopular = x.IsPopular,
                UpdatedAt = x.UpdatedAt ?? x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        if (minBoatCount.HasValue)
        {
            items = items.Where(x => x.TotalBoatCount >= minBoatCount.Value).ToList();
            totalCount = items.Count;
        }

        if (maxBoatCount.HasValue)
        {
            items = items.Where(x => x.TotalBoatCount <= maxBoatCount.Value).ToList();
            totalCount = items.Count;
        }

        var result = new PagedResult<AdminLocationListItemDto>(items, totalCount, page, pageSize);
        return Result<PagedResult<AdminLocationListItemDto>>.Success(result);
    }

    public async Task<Result<AdminLocationsSummaryDto>> GetAdminLocationsSummaryAsync(CancellationToken cancellationToken)
    {
        var locations = await _context.Locations.ToListAsync(cancellationToken);
        var harbors = await _context.Harbors.CountAsync(cancellationToken);

        var summary = new AdminLocationsSummaryDto
        {
            TotalLocations = locations.Count,
            ActiveLocations = locations.Count(x => x.Status == LocationStatus.Active),
            PassiveLocations = locations.Count(x => x.Status == LocationStatus.Passive),
            PopularLocations = locations.Count(x => x.IsPopular),
            TotalHarbors = harbors,
            SeoReadyLocations = locations.Count(x => x.SeoStatus == LocationSeoStatus.Ready),
            MissingCoordinates = locations.Count(x => x.CoordinateStatus == LocationCoordinateStatus.Missing),
            MissingCoverImages = locations.Count(x => string.IsNullOrEmpty(x.CoverImageUrl))
        };

        return Result<AdminLocationsSummaryDto>.Success(summary);
    }

    public async Task<Result<AdminLocationDetailDto>> GetLocationDetailAsync(Guid id, CancellationToken cancellationToken)
    {
        var location = await _context.Locations
            .Include(x => x.ParentLocation)
            .Include(x => x.Harbors)
            .Include(x => x.Boats)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (location == null)
            return Result<AdminLocationDetailDto>.Failure("NOT_FOUND", "Lokasyon bulunamadı.");

        var dto = new AdminLocationDetailDto
        {
            Id = location.Id,
            Name = location.Name,
            Slug = location.Slug,
            LocationType = location.Type.ToString(),
            ParentLocationId = location.ParentLocationId,
            ParentLocationName = location.ParentLocation?.Name,
            Region = location.Region.ToString(),
            Status = location.Status.ToString(),
            IsPopular = location.IsPopular,
            SortOrder = location.SortOrder,
            ShortDescription = location.ShortDescription,
            Description = location.Description,
            SeoTitle = location.SeoTitle,
            MetaDescription = location.MetaDescription,
            OgTitle = location.OgTitle,
            OgDescription = location.OgDescription,
            CanonicalUrl = location.CanonicalUrl,
            CoverImageUrl = location.CoverImageUrl,
            OgImageUrl = location.OgImageUrl,
            ImageAltText = location.ImageAltText,
            Address = location.Address,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            SeoStatus = location.SeoStatus.ToString(),
            CoordinateStatus = location.CoordinateStatus.ToString(),
            HarborCount = location.Harbors.Count,
            ActiveBoatCount = location.Boats.Count(b => b.Status == BoatStatus.Published),
            ActivePackageCount = 0,
            MonthlyReservationCount = 0,
            UpdatedAt = location.UpdatedAt ?? location.CreatedAt
        };

        return Result<AdminLocationDetailDto>.Success(dto);
    }

    public async Task<Result<Guid>> CreateLocationAsync(CreateLocationDto dto, Guid currentUserId, CancellationToken cancellationToken)
    {
        if (await _context.Locations.AnyAsync(x => x.Slug == dto.Slug, cancellationToken))
            return Result<Guid>.Failure("VALIDATION_ERROR", "Bu slug değeri zaten kullanımda.");

        var location = new Location
        {
            Name = dto.Name,
            Slug = dto.Slug,
            Type = dto.Type,
            Region = dto.Region,
            Status = dto.Status,
            ParentLocationId = dto.ParentLocationId,
            IsPopular = dto.IsPopular,
            Description = dto.Description,
            CreatedBy = currentUserId
        };

        _context.Locations.Add(location);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(location.Id);
    }

    public async Task<Result> UpdateLocationAsync(UpdateLocationDto dto, Guid currentUserId, CancellationToken cancellationToken)
    {
        var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);
        if (location == null)
            return Result.Failure("NOT_FOUND", "Lokasyon bulunamadı.");

        if (location.Slug != dto.Slug && await _context.Locations.AnyAsync(x => x.Slug == dto.Slug && x.Id != dto.Id, cancellationToken))
            return Result.Failure("VALIDATION_ERROR", "Bu slug değeri zaten kullanımda.");

        location.Name = dto.Name;
        location.Slug = dto.Slug;
        location.Type = dto.Type;
        location.Region = dto.Region;
        location.Status = dto.Status;
        location.ParentLocationId = dto.ParentLocationId;
        location.IsPopular = dto.IsPopular;
        location.Description = dto.Description;
        location.SortOrder = dto.SortOrder;
        location.ShortDescription = dto.ShortDescription;
        location.SeoTitle = dto.SeoTitle;
        location.MetaDescription = dto.MetaDescription;
        location.CanonicalUrl = dto.CanonicalUrl;
        location.OgTitle = dto.OgTitle;
        location.OgDescription = dto.OgDescription;
        location.OgImageUrl = dto.OgImageUrl;
        location.ImageAltText = dto.ImageAltText;
        location.CoverImageUrl = dto.CoverImageUrl;
        location.Address = dto.Address;
        location.Latitude = dto.Latitude;
        location.Longitude = dto.Longitude;
        location.UpdatedBy = currentUserId;

        // Auto update statuses
        location.CoordinateStatus = (dto.Latitude.HasValue && dto.Longitude.HasValue) ? LocationCoordinateStatus.Available : LocationCoordinateStatus.Missing;
        
        bool isSeoReady = !string.IsNullOrEmpty(dto.SeoTitle) && !string.IsNullOrEmpty(dto.MetaDescription) && !string.IsNullOrEmpty(dto.Slug);
        location.SeoStatus = isSeoReady ? LocationSeoStatus.Ready : LocationSeoStatus.Missing;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteLocationAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken)
    {
        var location = await _context.Locations
            .Include(x => x.Boats)
            .Include(x => x.Harbors)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            
        if (location == null)
            return Result.Failure("NOT_FOUND", "Lokasyon bulunamadı.");

        if (location.Boats.Any())
            return Result.Failure("VALIDATION_ERROR", "Bu lokasyona bağlı tekneler bulunduğu için silinemez.");

        // We can either delete harbors or block if there are harbors. Let's delete harbors since it's cascade in EF.
        _context.Locations.Remove(location);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ActivateLocationAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken)
    {
        var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (location == null) return Result.Failure("NOT_FOUND", "Lokasyon bulunamadı.");

        location.Status = LocationStatus.Active;
        location.UpdatedBy = currentUserId;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeactivateLocationAsync(Guid id, string? reason, Guid currentUserId, CancellationToken cancellationToken)
    {
        var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (location == null) return Result.Failure("NOT_FOUND", "Lokasyon bulunamadı.");

        location.Status = LocationStatus.Passive;
        location.UpdatedBy = currentUserId;
        // The reason is not saved to the DB right now since there's no field for it.
        // If we add LocationAdminNotes in the future, we could save it there.
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> MarkLocationPopularAsync(Guid id, string? note, int? order, Guid currentUserId, CancellationToken cancellationToken)
    {
        var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (location == null) return Result.Failure("NOT_FOUND", "Lokasyon bulunamadı.");

        location.IsPopular = true;
        if (order.HasValue) location.SortOrder = order.Value;
        location.UpdatedBy = currentUserId;
        // The note is ignored for now.

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UnmarkLocationPopularAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken)
    {
        var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (location == null) return Result.Failure("NOT_FOUND", "Lokasyon bulunamadı.");

        location.IsPopular = false;
        location.UpdatedBy = currentUserId;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<List<LocationHarborListItemDto>>> GetLocationHarborsAsync(Guid locationId, CancellationToken cancellationToken)
    {
        var harbors = await _context.Harbors
            .Where(x => x.LocationId == locationId)
            .Include(x => x.Boats)
            .Select(x => new LocationHarborListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type.ToString(),
                CoordinateStatus = x.CoordinateStatus.ToString(),
                ActiveBoatCount = x.Boats.Count(b => b.Status == BoatStatus.Published),
                Status = x.Status.ToString(),
                IsMainDeparturePoint = x.IsMainDeparturePoint,
                UpdatedAt = x.UpdatedAt ?? x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<List<LocationHarborListItemDto>>.Success(harbors);
    }

    public async Task<Result<Guid>> CreateHarborAsync(CreateHarborDto dto, Guid currentUserId, CancellationToken cancellationToken)
    {
        var harbor = new Harbor
        {
            LocationId = dto.LocationId,
            Name = dto.Name,
            Type = dto.Type,
            Status = dto.Status,
            IsMainDeparturePoint = dto.IsMainDeparturePoint,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            CoordinateStatus = (dto.Latitude.HasValue && dto.Longitude.HasValue) ? LocationCoordinateStatus.Available : LocationCoordinateStatus.Missing,
            CreatedBy = currentUserId
        };

        if (dto.IsMainDeparturePoint)
        {
            // Reset others
            var existingMain = await _context.Harbors.Where(x => x.LocationId == dto.LocationId && x.IsMainDeparturePoint).ToListAsync(cancellationToken);
            foreach (var h in existingMain)
            {
                h.IsMainDeparturePoint = false;
            }
        }

        _context.Harbors.Add(harbor);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(harbor.Id);
    }

    public async Task<Result> UpdateHarborAsync(UpdateHarborDto dto, Guid currentUserId, CancellationToken cancellationToken)
    {
        var harbor = await _context.Harbors.FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);
        if (harbor == null)
            return Result.Failure("NOT_FOUND", "Liman bulunamadı.");

        harbor.LocationId = dto.LocationId;
        harbor.Name = dto.Name;
        harbor.Type = dto.Type;
        harbor.Status = dto.Status;
        harbor.IsMainDeparturePoint = dto.IsMainDeparturePoint;
        harbor.Latitude = dto.Latitude;
        harbor.Longitude = dto.Longitude;
        harbor.CoordinateStatus = (dto.Latitude.HasValue && dto.Longitude.HasValue) ? LocationCoordinateStatus.Available : LocationCoordinateStatus.Missing;
        harbor.UpdatedBy = currentUserId;

        if (dto.IsMainDeparturePoint)
        {
            // Reset others
            var existingMain = await _context.Harbors.Where(x => x.LocationId == dto.LocationId && x.Id != dto.Id && x.IsMainDeparturePoint).ToListAsync(cancellationToken);
            foreach (var h in existingMain)
            {
                h.IsMainDeparturePoint = false;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteHarborAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken)
    {
        var harbor = await _context.Harbors
            .Include(x => x.Boats)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            
        if (harbor == null)
            return Result.Failure("NOT_FOUND", "Liman bulunamadı.");

        if (harbor.Boats.Any())
            return Result.Failure("VALIDATION_ERROR", "Bu limana bağlı tekneler bulunduğu için silinemez.");

        _context.Harbors.Remove(harbor);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
