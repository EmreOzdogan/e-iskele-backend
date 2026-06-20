using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EIskele.Application.Common.Errors;
using EIskele.Application.Common.Results;
using EIskele.Application.Packages;
using EIskele.Domain.Entities;
using EIskele.Domain.Enums;

namespace EIskele.Infrastructure.Services;

public partial class TourPackageService
{
    public async Task<Result<List<CaptainPackageListItemDto>>> GetMyPackagesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var packages = await _context.TourPackages
            .Include(p => p.Boat)
            .ThenInclude(b => b.Captain)
            .Include(p => p.Includes)
            .Include(p => p.Reservations)
            .Where(p => p.Boat.Captain.UserId == userId && !p.IsDeleted)
            .ToListAsync(cancellationToken);

        var dtos = packages.Select(p => new CaptainPackageListItemDto
        {
            Id = p.Id,
            Name = p.Name,
            BoatId = p.BoatId,
            BoatName = p.Boat.Name,
            BoatPublishStatus = p.Boat.Status.ToString(),
            TourType = p.TourType,
            Status = p.Status.ToString(),
            ApprovalType = p.ApprovalType.ToString(),
            DurationText = p.DurationHours > 0 ? $"{p.DurationHours} Saat" : "Belirtilmedi",
            StartTime = p.StartTime?.ToString(@"hh\:mm"),
            EndTime = p.EndTime?.ToString(@"hh\:mm"),
            MinGuests = p.MinCapacity,
            MaxGuests = p.MaxCapacity,
            PriceType = string.IsNullOrEmpty(p.Currency) ? "perPerson" : "wholeBoat", // temporary mapping
            Price = p.Price,
            DepositRate = p.PrepaymentPercentage,
            IncludedServices = p.Includes.Where(i => i.IsIncluded).Select(i => i.Name).ToList(),
            UpcomingReservationCount = p.Reservations.Count(r => r.Status == ReservationStatus.Approved || r.Status == ReservationStatus.PaymentPending || r.Status == ReservationStatus.Paid),
            LastUpdatedText = p.UpdatedAt?.ToString("dd MMM yyyy") ?? p.CreatedAt.ToString("dd MMM yyyy"),
            MissingItems = new List<string>() // Add missing items logic here if needed
        }).ToList();

        return Result<List<CaptainPackageListItemDto>>.Success(dtos);
    }

    public async Task<Result<CaptainPackageDetailDto>> GetMyPackageDetailAsync(Guid packageId, Guid userId, CancellationToken cancellationToken = default)
    {
        var package = await _context.TourPackages
            .Include(p => p.Boat)
            .ThenInclude(b => b.Captain)
            .Include(p => p.Includes)
            .Include(p => p.Reservations)
            .FirstOrDefaultAsync(p => p.Id == packageId && !p.IsDeleted, cancellationToken);

        if (package == null)
        {
            return Result<CaptainPackageDetailDto>.Failure(new Error("PackageError", "Paket bulunamadı."));
        }

        if (package.Boat.Captain.UserId != userId)
        {
            return Result<CaptainPackageDetailDto>.Failure(new Error("PackageError", "Bu paketi görüntüleme yetkiniz yok."));
        }

        var dto = new CaptainPackageDetailDto
        {
            Id = package.Id,
            Name = package.Name,
            BoatId = package.BoatId,
            BoatName = package.Boat.Name,
            BoatPublishStatus = package.Boat.Status.ToString(),
            BoatCapacity = package.Boat.Capacity,
            BoatLocation = package.Boat.LocationId.ToString(), // Need real location
            TourType = package.TourType,
            Status = package.Status.ToString(),
            ApprovalType = package.ApprovalType.ToString(),
            ReservationModel = "shared", // default
            ShortDescription = package.Description,
            Description = package.Description,
            Duration = package.DurationHours.ToString(),
            StartTime = package.StartTime?.ToString(@"hh\:mm") ?? "",
            EndTime = package.EndTime?.ToString(@"hh\:mm") ?? "",
            MinGuests = package.MinCapacity,
            MaxGuests = package.MaxCapacity,
            PriceType = "perPerson",
            PerPersonPrice = package.Price,
            WholeBoatPrice = package.Price * package.MaxCapacity,
            HasDeposit = package.PrepaymentPercentage > 0,
            DepositRate = package.PrepaymentPercentage,
            UseSeasonalPricing = false,
            IncludedServices = package.Includes.Where(i => i.IsIncluded).Select(i => i.Name).ToList(),
            CustomIncludedServices = new List<string>(),
            ExcludedServices = package.Includes.Where(i => !i.IsIncluded).Select(i => i.Name).ToList(),
            CustomExcludedServices = new List<string>(),
            CancellationPolicy = package.CancellationPolicyType,
            CustomCancellationPolicy = package.RefundPolicyNote,
            WeatherNote = package.WeatherPostponeNote,
            SpecialRules = package.CaptainCancellationNote,
            UpcomingReservationCount = package.Reservations.Count(r => r.Status == ReservationStatus.Approved || r.Status == ReservationStatus.PaymentPending || r.Status == ReservationStatus.Paid),
            CompletedReservationCount = package.Reservations.Count(r => r.Status == ReservationStatus.Completed),
            LastUpdatedText = package.UpdatedAt?.ToString("dd MMM yyyy") ?? package.CreatedAt.ToString("dd MMM yyyy"),
            MissingItems = new List<string>(),
            History = new List<PackageHistoryItemDto>()
        };

        return Result<CaptainPackageDetailDto>.Success(dto);
    }

    public async Task<Result<Guid>> CreateMyPackageAsync(Guid userId, CreateCaptainPackageRequest request, CancellationToken cancellationToken = default)
    {
        var boat = await _context.Boats
            .Include(b => b.Captain)
            .FirstOrDefaultAsync(b => b.Id == request.BoatId && !b.IsDeleted, cancellationToken);

        if (boat == null)
            return Result<Guid>.Failure(new Error("PackageError", "Tekne bulunamadı."));

        if (boat.Captain.UserId != userId)
            return Result<Guid>.Failure(new Error("PackageError", "Bu tekneye paket ekleme yetkiniz yok."));

        if (request.MinGuests > request.MaxGuests)
            return Result<Guid>.Failure(new Error("PackageError", "Minimum kişi sayısı maksimumdan büyük olamaz."));

        if (request.MaxGuests > boat.Capacity)
            return Result<Guid>.Failure(new Error("PackageError", "Paket kapasitesi tekne kapasitesini aşamaz."));

        if (!Enum.TryParse<ReservationApprovalType>(request.ApprovalType, true, out var approvalType))
            approvalType = ReservationApprovalType.AutoApprove;

        var package = new TourPackage
        {
            BoatId = request.BoatId,
            Name = request.Name,
            TourType = request.TourType,
            Description = request.Description ?? request.ShortDescription ?? string.Empty,
            MinCapacity = request.MinGuests,
            MaxCapacity = request.MaxGuests,
            IsActive = false, // starts as draft
            Status = TourPackageStatus.Draft,
            ApprovalType = approvalType,
            Price = request.PriceType == "wholeBoat" ? (request.WholeBoatPrice ?? 0) : (request.PerPersonPrice ?? 0),
            PrepaymentPercentage = request.HasDeposit ? (request.DepositRate ?? 0) : 0,
            CancellationPolicyType = request.CancellationPolicy,
            RefundPolicyNote = request.CustomCancellationPolicy,
            WeatherPostponeNote = request.WeatherNote,
            CaptainCancellationNote = request.SpecialRules
        };

        if (int.TryParse(request.Duration, out int durationHours))
            package.DurationHours = durationHours;
        
        if (TimeSpan.TryParse(request.StartTime, out var startTime))
            package.StartTime = startTime;
            
        if (TimeSpan.TryParse(request.EndTime, out var endTime))
            package.EndTime = endTime;

        foreach (var inc in request.IncludedServices)
            package.Includes.Add(new PackageInclude { Name = inc, IsIncluded = true });
        foreach (var inc in request.CustomIncludedServices)
            package.Includes.Add(new PackageInclude { Name = inc, IsIncluded = true });
        foreach (var exc in request.ExcludedServices)
            package.Includes.Add(new PackageInclude { Name = exc, IsIncluded = false });
        foreach (var exc in request.CustomExcludedServices)
            package.Includes.Add(new PackageInclude { Name = exc, IsIncluded = false });

        _context.TourPackages.Add(package);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(package.Id);
    }

    public async Task<Result> UpdateMyPackageAsync(Guid packageId, Guid userId, UpdateCaptainPackageRequest request, CancellationToken cancellationToken = default)
    {
        var package = await _context.TourPackages
            .Include(p => p.Boat)
            .ThenInclude(b => b.Captain)
            .Include(p => p.Includes)
            .FirstOrDefaultAsync(p => p.Id == packageId && !p.IsDeleted, cancellationToken);

        if (package == null)
            return Result.Failure(new Error("PackageError", "Paket bulunamadı."));

        if (package.Boat.Captain.UserId != userId)
            return Result.Failure(new Error("PackageError", "Bu paketi güncelleme yetkiniz yok."));

        if (request.MinGuests > request.MaxGuests)
            return Result.Failure(new Error("PackageError", "Minimum kişi sayısı maksimumdan büyük olamaz."));

        if (request.MaxGuests > package.Boat.Capacity)
            return Result.Failure(new Error("PackageError", "Paket kapasitesi tekne kapasitesini aşamaz."));

        if (Enum.TryParse<ReservationApprovalType>(request.ApprovalType, true, out var approvalType))
            package.ApprovalType = approvalType;

        package.Name = request.Name;
        package.TourType = request.TourType;
        package.Description = request.Description ?? request.ShortDescription ?? string.Empty;
        package.MinCapacity = request.MinGuests;
        package.MaxCapacity = request.MaxGuests;
        package.Price = request.PriceType == "wholeBoat" ? (request.WholeBoatPrice ?? 0) : (request.PerPersonPrice ?? 0);
        package.PrepaymentPercentage = request.HasDeposit ? (request.DepositRate ?? 0) : 0;
        package.CancellationPolicyType = request.CancellationPolicy;
        package.RefundPolicyNote = request.CustomCancellationPolicy;
        package.WeatherPostponeNote = request.WeatherNote;
        package.CaptainCancellationNote = request.SpecialRules;

        if (int.TryParse(request.Duration, out int durationHours))
            package.DurationHours = durationHours;
        
        if (TimeSpan.TryParse(request.StartTime, out var startTime))
            package.StartTime = startTime;
            
        if (TimeSpan.TryParse(request.EndTime, out var endTime))
            package.EndTime = endTime;

        _context.PackageIncludes.RemoveRange(package.Includes);
        
        var newIncludes = new List<PackageInclude>();
        foreach (var inc in request.IncludedServices)
            newIncludes.Add(new PackageInclude { TourPackageId = package.Id, Name = inc, IsIncluded = true });
        foreach (var inc in request.CustomIncludedServices)
            newIncludes.Add(new PackageInclude { TourPackageId = package.Id, Name = inc, IsIncluded = true });
        foreach (var exc in request.ExcludedServices)
            newIncludes.Add(new PackageInclude { TourPackageId = package.Id, Name = exc, IsIncluded = false });
        foreach (var exc in request.CustomExcludedServices)
            newIncludes.Add(new PackageInclude { TourPackageId = package.Id, Name = exc, IsIncluded = false });

        _context.PackageIncludes.AddRange(newIncludes);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ActivateMyPackageAsync(Guid packageId, Guid userId, CancellationToken cancellationToken = default)
    {
        var package = await _context.TourPackages
            .Include(p => p.Boat)
            .ThenInclude(b => b.Captain)
            .FirstOrDefaultAsync(p => p.Id == packageId && !p.IsDeleted, cancellationToken);

        if (package == null)
            return Result.Failure(new Error("PackageError", "Paket bulunamadı."));

        if (package.Boat.Captain.UserId != userId)
            return Result.Failure(new Error("PackageError", "Bu paketi aktifleştirme yetkiniz yok."));

        if (package.Boat.Status != BoatStatus.Published)
            return Result.Failure(new Error("PackageError", "Tekne yayında olmadan paket aktif edilemez."));

        package.Status = TourPackageStatus.Published;
        package.IsActive = true;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeactivateMyPackageAsync(Guid packageId, Guid userId, CancellationToken cancellationToken = default)
    {
        var package = await _context.TourPackages
            .Include(p => p.Boat)
            .ThenInclude(b => b.Captain)
            .FirstOrDefaultAsync(p => p.Id == packageId && !p.IsDeleted, cancellationToken);

        if (package == null)
            return Result.Failure(new Error("PackageError", "Paket bulunamadı."));

        if (package.Boat.Captain.UserId != userId)
            return Result.Failure(new Error("PackageError", "Bu paketi pasifleştirme yetkiniz yok."));

        package.Status = TourPackageStatus.Passive;
        package.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<Guid>> DuplicateMyPackageAsync(Guid packageId, Guid userId, CancellationToken cancellationToken = default)
    {
        var package = await _context.TourPackages
            .Include(p => p.Boat)
            .ThenInclude(b => b.Captain)
            .Include(p => p.Includes)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == packageId && !p.IsDeleted, cancellationToken);

        if (package == null)
            return Result<Guid>.Failure(new Error("PackageError", "Paket bulunamadı."));

        if (package.Boat.Captain.UserId != userId)
            return Result<Guid>.Failure(new Error("PackageError", "Bu paketi kopyalama yetkiniz yok."));

        package.Id = Guid.Empty;
        package.Name = package.Name + " (Kopya)";
        package.Status = TourPackageStatus.Draft;
        package.IsActive = false;
        
        foreach(var inc in package.Includes)
        {
            inc.Id = Guid.Empty;
            inc.TourPackageId = Guid.Empty;
        }

        _context.TourPackages.Add(package);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(package.Id);
    }

    public async Task<Result> DeleteMyPackageAsync(Guid packageId, Guid userId, CancellationToken cancellationToken = default)
    {
        var package = await _context.TourPackages
            .Include(p => p.Boat)
            .ThenInclude(b => b.Captain)
            .Include(p => p.Reservations)
            .FirstOrDefaultAsync(p => p.Id == packageId && !p.IsDeleted, cancellationToken);

        if (package == null)
            return Result.Failure(new Error("PackageError", "Paket bulunamadı."));

        if (package.Boat.Captain.UserId != userId)
            return Result.Failure(new Error("PackageError", "Bu paketi silme yetkiniz yok."));

        if (package.Reservations.Any(r => r.Status == ReservationStatus.Approved || r.Status == ReservationStatus.Paid || r.Status == ReservationStatus.PaymentPending))
            return Result.Failure(new Error("PackageError", "Aktif rezervasyonu olan paket silinemez."));

        package.IsDeleted = true;
        package.DeletedAt = DateTime.UtcNow;
        // In a real scenario we'd use current user's email/name for DeletedBy, leaving empty for now.
        
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
