using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EIskele.Application.Packages;
using EIskele.Application.Common.Results;
using EIskele.Application.Common.Errors;
using EIskele.Domain.Enums;
using EIskele.Domain.Entities;

namespace EIskele.Infrastructure.Services;

public partial class TourPackageService
{
    public async Task<Result<PackageDetailDto>> GetAdminPackageDetailAsync(Guid id)
    {
        var package = await _context.TourPackages
            .Include(x => x.Boat)
                .ThenInclude(b => b.Captain)
                    .ThenInclude(c => c.User)
            .Include(x => x.Boat)
                .ThenInclude(b => b.Location)
            .Include(x => x.Boat)
                .ThenInclude(b => b.Harbor)
            .Include(x => x.Includes)
            .Include(x => x.Reservations)
                .ThenInclude(r => r.Customer)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (package == null)
            return Result<PackageDetailDto>.Failure(new Error("Package.NotFound", "Paket bulunamadı."));

        var dto = new PackageDetailDto
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            TourType = package.TourType,
            Category = package.Category,
            DurationHours = package.DurationHours,
            StartTime = package.StartTime,
            EndTime = package.EndTime,
            TimeLabel = package.TimeLabel,
            Price = package.Price,
            PrepaymentPercentage = package.PrepaymentPercentage,
            ServiceFee = package.ServiceFee,
            Currency = package.Currency,
            MinCapacity = package.MinCapacity,
            MaxCapacity = package.MaxCapacity,
            IsChildFriendly = package.IsChildFriendly,
            CancellationPolicyType = package.CancellationPolicyType,
            FreeCancellationHours = package.FreeCancellationHours,
            CaptainCancellationNote = package.CaptainCancellationNote,
            WeatherPostponeNote = package.WeatherPostponeNote,
            RefundPolicyNote = package.RefundPolicyNote,
            Status = package.Status,
            ApprovalType = package.ApprovalType,
            CreatedAt = package.CreatedAt,
            UpdatedAt = package.UpdatedAt,
            BoatId = package.BoatId,
            BoatName = package.Boat?.Name ?? "",
            BoatCapacity = package.Boat?.Capacity ?? 0,
            BoatStatus = package.Boat?.Status.ToString() ?? "",
            Location = package.Boat?.Location?.Name ?? "",
            Harbor = package.Boat?.Harbor?.Name ?? "",
            CaptainId = package.Boat?.Captain?.Id ?? Guid.Empty,
            CaptainName = package.Boat?.Captain?.User != null ? package.Boat.Captain.User.FirstName + " " + package.Boat.Captain.User.LastName : "",
            CaptainStatus = package.Boat?.Captain?.Status == CaptainStatus.Approved ? "Approved" : "Pending",
            Includes = package.Includes.Select(i => new PackageIncludeDto
            {
                Id = i.Id,
                Name = i.Name,
                IsIncluded = i.IsIncluded,
                Description = i.Description,
                Status = i.Status.ToString()
            }).ToList(),
            RecentReservations = package.Reservations.OrderByDescending(r => r.CreatedAt).Take(5).Select(r => new PackageReservationDto
            {
                Id = r.Id,
                ReservationNumber = r.Id.ToString().Substring(0, 8), // mock representation
                CustomerName = r.Customer != null ? r.Customer.FirstName + " " + r.Customer.LastName : "Bilinmiyor",
                StartDateTime = r.StartDateTime,
                GuestCount = r.GuestCount,
                TotalPrice = r.TotalPrice,
                Status = r.Status.ToString()
            }).ToList()
        };

        return Result<PackageDetailDto>.Success(dto);
    }

    public async Task<Result> ApprovePackageAsync(Guid id)
    {
        var package = await _context.TourPackages.FindAsync(id);
        if (package == null) return Result.Failure(new Error("Package.NotFound", "Paket bulunamadı."));

        package.Status = TourPackageStatus.Published;
        package.IsActive = true;
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> RejectPackageAsync(Guid id, string reason)
    {
        var package = await _context.TourPackages.FindAsync(id);
        if (package == null) return Result.Failure(new Error("Package.NotFound", "Paket bulunamadı."));

        package.Status = TourPackageStatus.Rejected;
        package.IsActive = false;
        // You might want to save the reason somewhere, e.g. UserAdminNote
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> RequestRevisionAsync(Guid id, string[] fields, string note)
    {
        var package = await _context.TourPackages.FindAsync(id);
        if (package == null) return Result.Failure(new Error("Package.NotFound", "Paket bulunamadı."));

        package.Status = TourPackageStatus.Draft; // Or a specific RevisionRequested state
        package.IsActive = false;
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> DeactivatePackageAsync(Guid id, string reason)
    {
        var package = await _context.TourPackages.FindAsync(id);
        if (package == null) return Result.Failure(new Error("Package.NotFound", "Paket bulunamadı."));

        package.Status = TourPackageStatus.Passive;
        package.IsActive = false;
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> SuspendPackageAsync(Guid id, string reason)
    {
        var package = await _context.TourPackages.FindAsync(id);
        if (package == null) return Result.Failure(new Error("Package.NotFound", "Paket bulunamadı."));

        package.Status = TourPackageStatus.Suspended;
        package.IsActive = false;
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> ReactivatePackageAsync(Guid id)
    {
        var package = await _context.TourPackages.FindAsync(id);
        if (package == null) return Result.Failure(new Error("Package.NotFound", "Paket bulunamadı."));

        package.Status = TourPackageStatus.Published;
        package.IsActive = true;
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> AddAdminNoteAsync(Guid id, string noteType, string noteText)
    {
        var package = await _context.TourPackages.FindAsync(id);
        if (package == null) return Result.Failure(new Error("Package.NotFound", "Paket bulunamadı."));

        // Here we create a note in UserAdminNote or a new PackageAdminNote entity.
        // For MVP, we can just return success or save to existing notes mechanism.
        return Result.Success();
    }

    public async Task<Result<List<AdminTourPackageListItemDto>>> GetAdminPackagesAsync(string? status, string? search, Guid? boatId, CancellationToken cancellationToken = default)
    {
        var query = _context.TourPackages
            .Include(p => p.Boat)
                .ThenInclude(b => b.Captain)
                    .ThenInclude(c => c.User)
            .Include(p => p.Boat)
                .ThenInclude(b => b.Location)
            .Include(p => p.Boat)
                .ThenInclude(b => b.Harbor)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            if (Enum.TryParse<TourPackageStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(p => p.Status == parsedStatus);
            }
        }

        if (boatId.HasValue)
        {
            query = query.Where(p => p.BoatId == boatId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(search) || 
                                     (p.Boat != null && p.Boat.Name.ToLower().Contains(search)));
        }

        var packages = await query.OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);

        var dtos = packages.Select(p => new AdminTourPackageListItemDto
        {
            Id = p.Id,
            PackageNo = p.Id.ToString().Substring(0, 8).ToUpper(),
            PackageName = p.Name,
            ShortDescription = p.Description,
            BoatId = p.BoatId,
            BoatName = p.Boat?.Name ?? "",
            BoatType = p.Boat?.BoatType ?? "",
            BoatPublishStatus = p.Boat?.Status.ToString() ?? "",
            CaptainId = p.Boat?.Captain?.Id ?? Guid.Empty,
            CaptainName = p.Boat?.Captain?.User != null ? $"{p.Boat.Captain.User.FirstName} {p.Boat.Captain.User.LastName}" : "",
            CaptainApplicationType = p.Boat?.Captain?.ApplicationType.ToString() ?? "",
            CaptainStatus = p.Boat?.Captain?.Status.ToString() ?? "",
            Location = p.Boat?.Location?.Name ?? "",
            Harbor = p.Boat?.Harbor?.Name ?? "",
            TourType = p.TourType,
            Price = p.Price,
            MinGuests = p.MinCapacity,
            MaxGuests = p.MaxCapacity,
            DurationText = p.DurationHours > 0 ? $"{p.DurationHours} Saat" : "Belirtilmemiş",
            PackageStatus = p.Status.ToString(),
            SubmittedForReviewAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt ?? p.CreatedAt
        }).ToList();

        return Result<List<AdminTourPackageListItemDto>>.Success(dtos);
    }

    public async Task<Result<PackageStatsDto>> GetAdminPackageStatsAsync(CancellationToken cancellationToken = default)
    {
        var packages = await _context.TourPackages.ToListAsync(cancellationToken);
        
        var stats = new PackageStatsDto
        {
            TotalCount = packages.Count,
            ActiveCount = packages.Count(p => p.Status == TourPackageStatus.Published),
            InReviewCount = packages.Count(p => p.Status == TourPackageStatus.UnderReview || p.Status == TourPackageStatus.Draft),
            PassiveCount = packages.Count(p => p.Status == TourPackageStatus.Passive),
            RejectedCount = packages.Count(p => p.Status == TourPackageStatus.Rejected),
            RevisionRequestedCount = 0, // Packages don't have a specific MissingDocument status
            FeaturedCount = 0,
            AveragePrice = packages.Any() ? packages.Average(p => p.Price) : 0
        };

        return Result<PackageStatsDto>.Success(stats);
    }
}
