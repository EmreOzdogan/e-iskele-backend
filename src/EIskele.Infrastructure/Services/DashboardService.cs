using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EIskele.Application.Common.Results;
using EIskele.Application.Dashboard;
using EIskele.Persistence;
using EIskele.Domain.Enums;
using System.Collections.Generic;

namespace EIskele.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly EIskeleDbContext _context;

    public DashboardService(EIskeleDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AdminDashboardSummaryResponseDto>> GetAdminDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        var response = new AdminDashboardSummaryResponseDto();

        var usersCount = await _context.Users.CountAsync(cancellationToken);
        var captainsCount = await _context.Captains.CountAsync(cancellationToken);
        var boatsCount = await _context.Boats.CountAsync(cancellationToken);
        var activeBoatsCount = await _context.Boats.CountAsync(b => b.Status == BoatStatus.Published, cancellationToken);
        var pendingCaptainsCount = await _context.Captains.CountAsync(a => a.Status == "UnderReview", cancellationToken);
        var pendingBoatsCount = await _context.Boats.CountAsync(b => b.Status == BoatStatus.UnderReview, cancellationToken);
        
        var today = DateTime.UtcNow.Date;
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        
        var todayReservationsCount = await _context.Reservations.CountAsync(r => r.CreatedAt >= today, cancellationToken);
        var monthlyRevenue = await _context.Payments.Where(p => p.Status == PaymentStatus.Paid && p.CreatedAt >= startOfMonth).SumAsync(p => p.Amount, cancellationToken);

        response.Metrics = new List<DashboardMetricDto>
        {
            new DashboardMetricDto { Id = "m1", Title = "Toplam Kullanıcı", Value = usersCount.ToString(), Description = "Sisteme kayıtlı toplam üye sayısı.", Icon = "users" },
            new DashboardMetricDto { Id = "m2", Title = "Toplam Kaptan", Value = captainsCount.ToString(), Description = "Onaylı ve aktif kaptan sayısı.", Icon = "anchor" },
            new DashboardMetricDto { Id = "m3", Title = "Toplam Tekne", Value = boatsCount.ToString(), Description = "Sistemde kayıtlı toplam tekne.", Icon = "ship" },
            new DashboardMetricDto { Id = "m4", Title = "Aktif Tekne", Value = activeBoatsCount.ToString(), Description = "Müşterilere listelenen açık tekneler.", Icon = "check-circle" },
            new DashboardMetricDto { Id = "m5", Title = "Bekleyen Başvurular", Value = pendingCaptainsCount.ToString(), Description = "İncelenmesi gereken kaptan başvuruları.", Icon = "file-text", ChangeType = pendingCaptainsCount > 0 ? "warning" : "neutral", Href = "/kaptanlar/basvurular" },
            new DashboardMetricDto { Id = "m6", Title = "Onay Bekleyen Tekneler", Value = pendingBoatsCount.ToString(), Description = "Yayına alınmak için kontrol bekleyen tekneler.", Icon = "alert-circle", ChangeType = pendingBoatsCount > 0 ? "warning" : "neutral", Href = "/tekneler/onay-bekleyenler" },
            new DashboardMetricDto { Id = "m7", Title = "Bugünkü Rezervasyonlar", Value = todayReservationsCount.ToString(), Description = "Bugün oluşturulan rezervasyonlar.", Icon = "calendar", Href = "/rezervasyonlar?date=today" },
            new DashboardMetricDto { Id = "m8", Title = "Aylık Ciro", Value = $"₺{monthlyRevenue:N2}", Description = "Bu ayki toplam ödemeler.", Icon = "credit-card" }
        };

        if (pendingCaptainsCount > 0)
        {
            response.OperationAlerts.Add(new OperationAlertDto { Id = "a1", Title = $"{pendingCaptainsCount} kaptan başvurusu inceleme bekliyor.", Description = "Gecikmiş değerlendirmeler olabilir.", Priority = "high", Href = "/kaptanlar/basvurular" });
        }
        if (pendingBoatsCount > 0)
        {
            response.OperationAlerts.Add(new OperationAlertDto { Id = "a2", Title = $"{pendingBoatsCount} tekne yayın onayı bekliyor.", Description = "Tekne görsellerini ve ruhsatlarını kontrol edin.", Priority = "high", Href = "/tekneler/onay-bekleyenler" });
        }

        // Reservation Trend
        for (int i = 6; i >= 0; i--)
        {
            var date = today.AddDays(-i);
            var res = await _context.Reservations.CountAsync(r => r.CreatedAt >= date && r.CreatedAt < date.AddDays(1), cancellationToken);
            var canc = await _context.Reservations.CountAsync(r => r.Status == ReservationStatus.Cancelled && r.CreatedAt >= date && r.CreatedAt < date.AddDays(1), cancellationToken);
            response.ReservationTrend.Add(new ReservationTrendDto { Day = date.ToString("ddd"), Reservations = res, Cancelled = canc });
        }

        // Recent Reservations
        var recent = await _context.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Boat)
                .ThenInclude(b => b.Captain)
                .ThenInclude(c => c.User)
            .Include(r => r.Boat.Location)
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        foreach (var r in recent)
        {
            response.RecentReservations.Add(new RecentReservationDto
            {
                Id = r.Id.ToString(),
                ReservationNo = r.ReservationNo,
                CustomerName = r.Customer.FirstName + " " + r.Customer.LastName,
                CaptainName = r.Boat.Captain.User.FirstName + " " + r.Boat.Captain.User.LastName,
                BoatName = r.Boat.Name,
                Location = r.Boat.Location.Name,
                Date = r.StartDateTime.ToString("dd MMM yyyy"),
                Amount = r.TotalPrice,
                Status = r.Status.ToString()
            });
        }

        // Platform Health (Mock for now)
        response.PlatformHealth = new List<PlatformHealthItemDto>
        {
            new PlatformHealthItemDto { Id = "ph1", Label = "API Durumu", Status = "active" },
            new PlatformHealthItemDto { Id = "ph2", Label = "Bildirim Servisi", Status = "active" },
            new PlatformHealthItemDto { Id = "ph3", Label = "Dosya Yükleme", Status = "active" }
        };

        // Finance
        var platformCommission = await _context.Payments.Where(p => p.Status == PaymentStatus.Paid && p.CreatedAt >= startOfMonth).SumAsync(p => p.PlatformCommission, cancellationToken);
        var pendingPayout = await _context.Payments.Where(p => p.PayoutStatus == PayoutStatus.Pending).SumAsync(p => p.CaptainEarnings, cancellationToken);
        var refundRequests = await _context.Payments.CountAsync(p => p.RefundStatus == RefundStatus.Pending, cancellationToken);
        var avgAmount = await _context.Payments.Where(p => p.Status == PaymentStatus.Paid).AverageAsync(p => (decimal?)p.Amount, cancellationToken) ?? 0;

        response.FinanceSummary = new FinanceSummaryDto
        {
            MonthlyRevenue = monthlyRevenue,
            PlatformCommission = platformCommission,
            PendingPayout = pendingPayout,
            RefundRequests = refundRequests,
            AverageReservationAmount = avgAmount
        };

        return Result<AdminDashboardSummaryResponseDto>.Success(response);
    }
}
