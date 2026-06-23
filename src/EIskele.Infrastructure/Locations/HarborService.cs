using EIskele.Application.Common.Locations;
using EIskele.Application.Common.Results;
using EIskele.Domain.Entities;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Locations;

public class HarborService : IHarborService
{
    private readonly EIskeleDbContext _context;

    public HarborService(EIskeleDbContext context)
    {
        _context = context;
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

    public async Task<Result<List<ActiveHarborDto>>> GetActiveHarborsAsync(CancellationToken cancellationToken)
    {
        var harbors = await _context.Harbors
            .Where(x => x.Status == LocationStatus.Active && x.Location.Status == LocationStatus.Active)
            .OrderBy(x => x.Name)
            .Select(x => new ActiveHarborDto
            {
                Id = x.Id,
                LocationId = x.LocationId,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);

        return Result<List<ActiveHarborDto>>.Success(harbors);
    }
}
