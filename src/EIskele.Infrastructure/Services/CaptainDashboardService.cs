using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Dashboard;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Services;

public class CaptainDashboardService : ICaptainDashboardService
{
    private readonly EIskeleDbContext _context;

    public CaptainDashboardService(EIskeleDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CaptainDashboardDataDto>> GetDashboardSummaryAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userId, out var userGuid))
            return Result<CaptainDashboardDataDto>.Failure(new EIskele.Application.Common.Errors.Error("User.InvalidId", "Geçersiz kullanıcı ID'si."));

        var captain = await _context.Captains
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == userGuid, cancellationToken);

        if (captain == null)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userGuid, cancellationToken);
            if (user == null)
                return Result<CaptainDashboardDataDto>.Failure(new EIskele.Application.Common.Errors.Error("User.NotFound", "Kullanıcı bulunamadı."));

            return Result<CaptainDashboardDataDto>.Success(new CaptainDashboardDataDto
            {
                CaptainName = $"{user.FirstName} {user.LastName}",
                TodayText = DateTime.UtcNow.ToString("dd MMMM yyyy, dddd"),
                OperationStatus = "limited",
                ProfileCompletionRate = 20,
                Metrics = new List<CaptainDashboardMetricDto>
                {
                    new CaptainDashboardMetricDto { Key = "todayReservations", Title = "Bugünkü Rezervasyonlar", Value = 0, Description = "Bugün planlanan tur", Status = "info" },
                    new CaptainDashboardMetricDto { Key = "pendingRequests", Title = "Bekleyen Talepler", Value = 0, Description = "Yanıt bekleyen rezervasyon", Status = "success" },
                    new CaptainDashboardMetricDto { Key = "monthlyEarnings", Title = "Aylık Kazanç", Value = "₺0", Description = "Bu ay tahmini kazanç", Status = "success" },
                    new CaptainDashboardMetricDto { Key = "activeBoats", Title = "Aktif Tekneler", Value = 0, Description = "Yayındaki tekneleriniz", Status = "default" }
                },
                TodayReservations = new List<CaptainDashboardReservationDto>(),
                PendingReservations = new List<CaptainDashboardReservationDto>(),
                BoatStatuses = new List<CaptainDashboardBoatStatusDto>(),
                RecentReviews = new List<CaptainDashboardReviewDto>()
            });
        }

        var response = new CaptainDashboardDataDto
        {
            CaptainName = $"{captain.User.FirstName} {captain.User.LastName}",
            TodayText = DateTime.UtcNow.ToString("dd MMMM yyyy, dddd"),
            OperationStatus = captain.Status == CaptainStatus.Approved ? "active" : "limited",
            ProfileCompletionRate = string.IsNullOrEmpty(captain.Iban) ? 80 : 100
        };

        var today = DateTime.UtcNow.Date;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);

        // Fetch boats
        var boats = await _context.Boats
            .Include(b => b.TourPackages)
            .Where(b => b.CaptainId == captain.Id && !b.IsDeleted)
            .ToListAsync(cancellationToken);

        // Fetch reservations
        var reservations = await _context.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Boat)
            .Include(r => r.TourPackage)
            .Where(r => r.Boat.CaptainId == captain.Id)
            .ToListAsync(cancellationToken);

        // Metrics
        var todayReservationsCount = reservations.Count(r => r.StartDateTime.Date == today && r.Status != ReservationStatus.Cancelled && r.Status != ReservationStatus.Rejected);
        var pendingRequestsCount = reservations.Count(r => r.Status == ReservationStatus.WaitingCaptainApproval);
        var monthlyEarnings = await _context.Payments
            .Where(p => p.Reservation.Boat.CaptainId == captain.Id && p.Status == PaymentStatus.Paid && p.CreatedAt >= startOfMonth)
            .SumAsync(p => p.CaptainEarnings, cancellationToken);
        var activeBoats = boats.Count(b => b.Status == BoatStatus.Published);

        response.Metrics = new List<CaptainDashboardMetricDto>
        {
            new CaptainDashboardMetricDto { Key = "todayReservations", Title = "Bugünkü Rezervasyonlar", Value = todayReservationsCount, Description = "Bugün planlanan tur", Status = "info" },
            new CaptainDashboardMetricDto { Key = "pendingRequests", Title = "Bekleyen Talepler", Value = pendingRequestsCount, Description = "Yanıt bekleyen rezervasyon", Status = pendingRequestsCount > 0 ? "warning" : "success" },
            new CaptainDashboardMetricDto { Key = "monthlyEarnings", Title = "Aylık Kazanç", Value = $"₺{monthlyEarnings:N0}", Description = "Bu ay tahmini kazanç", Status = "success" },
            new CaptainDashboardMetricDto { Key = "activeBoats", Title = "Aktif Tekneler", Value = activeBoats, Description = "Yayındaki tekneleriniz", Status = "default" }
        };

        // Today Reservations
        var todayRes = reservations.Where(r => r.StartDateTime.Date == today && r.Status != ReservationStatus.Cancelled && r.Status != ReservationStatus.Rejected).OrderBy(r => r.StartDateTime).ToList();
        response.TodayReservations = todayRes.Select(r => new CaptainDashboardReservationDto
        {
            Id = r.Id.ToString(),
            ReservationNo = r.ReservationNo,
            CustomerName = r.Customer.FirstName + " " + r.Customer.LastName,
            BoatName = r.Boat.Name,
            PackageName = r.TourPackage.Name,
            TourTitle = r.TourPackage.Name,
            Date = r.StartDateTime.ToString("yyyy-MM-dd"),
            TimeRange = $"{r.StartDateTime:HH:mm} - {r.EndDateTime:HH:mm}",
            GuestCount = r.GuestCount,
            Amount = r.TotalPrice,
            Status = r.Status.ToString(),
            PaymentStatus = "paid" // Mock payment status for now until payment fully implemented
        }).ToList();

        // Pending Reservations
        var pendingRes = reservations.Where(r => r.Status == ReservationStatus.WaitingCaptainApproval).OrderBy(r => r.CreatedAt).ToList();
        response.PendingReservations = pendingRes.Select(r => new CaptainDashboardReservationDto
        {
            Id = r.Id.ToString(),
            ReservationNo = r.ReservationNo,
            CustomerName = r.Customer.FirstName + " " + r.Customer.LastName,
            BoatName = r.Boat.Name,
            PackageName = r.TourPackage.Name,
            TourTitle = r.TourPackage.Name,
            Date = r.StartDateTime.ToString("yyyy-MM-dd"),
            TimeRange = $"{r.StartDateTime:HH:mm} - {r.EndDateTime:HH:mm}",
            GuestCount = r.GuestCount,
            Amount = r.TotalPrice,
            Status = "waitingCaptainApproval",
            PaymentStatus = "pending"
        }).ToList();

        // Boat Statuses
        response.BoatStatuses = boats.Select(b => new CaptainDashboardBoatStatusDto
        {
            Id = b.Id.ToString(),
            BoatName = b.Name,
            Status = b.Status.ToString().First().ToString().ToLower() + b.Status.ToString().Substring(1),
            ActivePackageCount = b.TourPackages.Count(p => p.IsActive),
            CalendarStatus = "updated", // mock calendar status
            LastUpdatedText = b.UpdatedAt?.ToString("dd MMM yyyy") ?? b.CreatedAt.ToString("dd MMM yyyy")
        }).ToList();

        // Recent Reviews
        var recentReviews = await _context.Reviews
            .Include(r => r.Customer)
            .Where(r => r.Reservation.Boat.CaptainId == captain.Id)
            .OrderByDescending(r => r.CreatedAt)
            .Take(3)
            .ToListAsync(cancellationToken);

        response.RecentReviews = recentReviews.Select(r => new CaptainDashboardReviewDto
        {
            Id = r.Id.ToString(),
            CustomerName = r.Customer.FirstName + " " + r.Customer.LastName,
            Rating = r.Rating,
            Comment = r.Comment,
            DateText = r.CreatedAt.ToString("dd MMMM yyyy")
        }).ToList();

        return Result<CaptainDashboardDataDto>.Success(response);
    }
}
