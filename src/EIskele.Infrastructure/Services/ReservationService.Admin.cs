using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Reservations;
using EIskele.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Services;

public partial class ReservationService
{
    public async Task<Result<AdminReservationSummaryMetricsDto>> GetAdminReservationsSummaryAsync(CancellationToken cancellationToken = default)
    {
        var reservations = _dbContext.Reservations.AsNoTracking();

        var total = await reservations.CountAsync(cancellationToken);
        
        var todayStart = DateTime.UtcNow.Date;
        var todayEnd = todayStart.AddDays(1);
        var todayCount = await reservations.CountAsync(r => r.CreatedAt >= todayStart && r.CreatedAt < todayEnd, cancellationToken);
        
        var waitingCount = await reservations.CountAsync(r => r.Status == ReservationStatus.WaitingCaptainApproval, cancellationToken);
        var paymentPendingCount = await reservations.CountAsync(r => r.Status == ReservationStatus.PaymentPending, cancellationToken);
        var approvedCount = await reservations.CountAsync(r => r.Status == ReservationStatus.Approved, cancellationToken);
        var completedCount = await reservations.CountAsync(r => r.Status == ReservationStatus.Completed, cancellationToken);
        var cancelledCount = await reservations.CountAsync(r => r.Status == ReservationStatus.Cancelled, cancellationToken);
        var postponedCount = await reservations.CountAsync(r => r.Status == ReservationStatus.PostponedDueToWeather, cancellationToken);

        return Result<AdminReservationSummaryMetricsDto>.Success(new AdminReservationSummaryMetricsDto
        {
            TotalCount = total,
            TodayCount = todayCount,
            WaitingCaptainCount = waitingCount,
            PaymentPendingCount = paymentPendingCount,
            ApprovedCount = approvedCount,
            CompletedCount = completedCount,
            CancelledCount = cancelledCount,
            PostponedCount = postponedCount
        });
    }

    public async Task<Result<PagedResult<AdminReservationListItemDto>>> GetAdminReservationsAsync(GetAdminReservationsQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Boat)
            .ThenInclude(b => b.Captain)
            .ThenInclude(c => c.User)
            .Include(r => r.TourPackage)
            .AsNoTracking();

        // Search
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            queryable = queryable.Where(r => 
                r.ReservationNo.ToLower().Contains(search) ||
                r.Customer.FirstName.ToLower().Contains(search) ||
                r.Customer.LastName.ToLower().Contains(search) ||
                r.Boat.Name.ToLower().Contains(search));
        }

        // Filters
        if (query.CustomerId.HasValue)
            queryable = queryable.Where(r => r.CustomerId == query.CustomerId.Value);

        if (query.CaptainId.HasValue)
            queryable = queryable.Where(r => r.Boat.CaptainId == query.CaptainId.Value);

        if (query.BoatId.HasValue)
            queryable = queryable.Where(r => r.BoatId == query.BoatId.Value);

        if (!string.IsNullOrWhiteSpace(query.ReservationStatus))
        {
            if (Enum.TryParse<ReservationStatus>(query.ReservationStatus, true, out var statusEnum))
            {
                queryable = queryable.Where(r => r.Status == statusEnum);
            }
        }

        if (query.DateFrom.HasValue)
            queryable = queryable.Where(r => r.StartDateTime >= query.DateFrom.Value);

        if (query.DateTo.HasValue)
            queryable = queryable.Where(r => r.StartDateTime <= query.DateTo.Value);

        var totalRecords = await queryable.CountAsync(cancellationToken);

        // Sorting
        queryable = query.SortBy?.ToLower() switch
        {
            "date" => query.SortDirection?.ToLower() == "asc" ? queryable.OrderBy(r => r.StartDateTime) : queryable.OrderByDescending(r => r.StartDateTime),
            "amount" => query.SortDirection?.ToLower() == "asc" ? queryable.OrderBy(r => r.TotalPrice) : queryable.OrderByDescending(r => r.TotalPrice),
            _ => queryable.OrderByDescending(r => r.CreatedAt)
        };

        var items = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(r => new AdminReservationListItemDto
            {
                Id = r.Id,
                ReservationNo = r.ReservationNo,
                Source = "web",
                CreatedAt = r.CreatedAt,
                CustomerId = r.CustomerId,
                CustomerName = r.Customer.FirstName + " " + r.Customer.LastName,
                CustomerPhone = r.Customer.PhoneNumber ?? "",
                CustomerEmail = r.Customer.Email,
                CaptainId = r.Boat.CaptainId,
                CaptainName = r.Boat.Captain.User.FirstName + " " + r.Boat.Captain.User.LastName,
                BoatId = r.BoatId,
                BoatName = r.Boat.Name,
                BoatPublishStatus = r.Boat.Status.ToString(),
                PackageId = r.TourPackageId,
                PackageName = r.PackageNameSnapshot ?? r.TourPackage.Name,
                TourType = "Balık Turu",
                Location = r.Boat.Location != null ? r.Boat.Location.Name : "",
                Harbor = r.Boat.Harbor != null ? r.Boat.Harbor.Name : "",
                TourDate = r.StartDateTime.Date,
                StartTime = r.StartDateTime.ToString("HH:mm"),
                EndTime = r.EndDateTime.ToString("HH:mm"),
                GuestCount = r.GuestCount,
                TotalAmount = r.TotalPrice,
                ApprovalType = r.TourPackage.ApprovalType.ToString(),
                ReservationStatus = r.Status.ToString(),
                PaymentStatus = "pending" // TODO: Payment tablosu eklendiğinde buradan okunacak
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<AdminReservationListItemDto>>.Success(new PagedResult<AdminReservationListItemDto>(items, totalRecords, query.Page, query.PageSize));
    }

    public async Task<Result<AdminReservationDetailDto>> GetAdminReservationDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reservation = await _dbContext.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Boat)
            .ThenInclude(b => b.Captain)
            .ThenInclude(c => c.User)
            .Include(r => r.Boat.Location)
            .Include(r => r.Boat.Harbor)
            .Include(r => r.TourPackage)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (reservation == null)
            return Result<AdminReservationDetailDto>.Failure("NOT_FOUND", "Rezervasyon bulunamadı.");

        var dto = new AdminReservationDetailDto
        {
            Id = reservation.Id,
            ReservationNo = reservation.ReservationNo,
            Source = "web",
            CreatedAt = reservation.CreatedAt,
            UpdatedAt = reservation.UpdatedAt ?? reservation.CreatedAt,
            CustomerId = reservation.CustomerId,
            CustomerName = reservation.Customer.FirstName + " " + reservation.Customer.LastName,
            CustomerPhone = reservation.Customer.PhoneNumber ?? "",
            CustomerEmail = reservation.Customer.Email,
            CaptainId = reservation.Boat.CaptainId,
            CaptainName = reservation.Boat.Captain.User.FirstName + " " + reservation.Boat.Captain.User.LastName,
            BoatId = reservation.BoatId,
            BoatName = reservation.Boat.Name,
            BoatPublishStatus = reservation.Boat.Status.ToString(),
            PackageId = reservation.TourPackageId,
            PackageName = reservation.PackageNameSnapshot ?? reservation.TourPackage.Name,
            TourType = "Balık Turu",
            Location = reservation.Boat.Location?.Name ?? "",
            Harbor = reservation.Boat.Harbor?.Name ?? "",
            TourDate = reservation.StartDateTime.Date,
            StartTime = reservation.StartDateTime.ToString("HH:mm"),
            EndTime = reservation.EndDateTime.ToString("HH:mm"),
            DurationText = $"{(reservation.EndDateTime - reservation.StartDateTime).TotalHours} Saat",
            GuestCount = reservation.GuestCount,
            TotalAmount = reservation.TotalPrice,
            ApprovalType = reservation.TourPackage.ApprovalType.ToString(),
            ReservationStatus = reservation.Status.ToString(),
            PaymentStatus = "pending", // TODO: Read from Payments
            
            CustomerNote = "Mock customer note", // TODO: Add column
            SpecialRequest = null,
            HealthNote = null,

            ServiceFee = 0,
            DiscountAmount = 0,
            DepositAmount = 0,
            RemainingAmount = reservation.TotalPrice,

            PaymentInfo = new ReservationPaymentInfoDto
            {
                TotalAmount = reservation.TotalPrice,
                PaymentStatus = "pending"
            },
            CancellationInfo = new ReservationCancellationInfoDto
            {
                IsCancelled = reservation.Status == ReservationStatus.Cancelled
            }
        };

        return Result<AdminReservationDetailDto>.Success(dto);
    }

    public async Task<Result> AdminCancelReservationAsync(Guid id, string reason, CancellationToken cancellationToken = default)
    {
        var reservation = await _dbContext.Reservations
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
            
        if (reservation == null)
            return Result.Failure("NOT_FOUND", "Rezervasyon bulunamadı.");

        reservation.Status = ReservationStatus.Cancelled;
        
        // Send email
        if (reservation.Customer != null && !string.IsNullOrWhiteSpace(reservation.Customer.Email))
        {
            var smtpSettingsResult = await _smtpEmailSettingsService.GetSmtpEmailSettingsAsync(cancellationToken);
            if (smtpSettingsResult.IsSuccess && smtpSettingsResult.Value != null && smtpSettingsResult.Value.SmtpEnabled)
            {
                var htmlBody = $@"
                    <h2>Rezervasyonunuz İptal Edildi</h2>
                    <p>Sayın {reservation.Customer.FirstName},</p>
                    <p><strong>{reservation.ReservationNo}</strong> numaralı rezervasyonunuz iptal edilmiştir.</p>
                    <p><strong>İptal Sebebi:</strong> {reason}</p>
                    <p>İyi günler dileriz.</p>";

                try
                {
                    await _emailSender.SendAsync(
                        reservation.Customer.Email, 
                        "e-iskele: Rezervasyonunuz İptal Edildi", 
                        htmlBody, 
                        smtpSettingsResult.Value, 
                        null, 
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    // Log exception, but don't fail the cancellation
                    Console.WriteLine($"Email send failed: {ex.Message}");
                }
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> AdminPostponeReservationAsync(Guid id, DateTime newDate, CancellationToken cancellationToken = default)
    {
        var reservation = await _dbContext.Reservations
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
            
        if (reservation == null)
            return Result.Failure("NOT_FOUND", "Rezervasyon bulunamadı.");

        var duration = reservation.EndDateTime - reservation.StartDateTime;
        reservation.StartDateTime = newDate;
        reservation.EndDateTime = newDate.Add(duration);
        reservation.Status = ReservationStatus.PostponedDueToWeather; 
        
        // Send email
        if (reservation.Customer != null && !string.IsNullOrWhiteSpace(reservation.Customer.Email))
        {
            var smtpSettingsResult = await _smtpEmailSettingsService.GetSmtpEmailSettingsAsync(cancellationToken);
            if (smtpSettingsResult.IsSuccess && smtpSettingsResult.Value != null && smtpSettingsResult.Value.SmtpEnabled)
            {
                var htmlBody = $@"
                    <h2>Rezervasyonunuz Ertelendi</h2>
                    <p>Sayın {reservation.Customer.FirstName},</p>
                    <p><strong>{reservation.ReservationNo}</strong> numaralı rezervasyonunuzun tarihi güncellenmiştir.</p>
                    <p><strong>Yeni Tarih:</strong> {newDate.ToString("dd.MM.yyyy HH:mm")}</p>
                    <p>İyi günler dileriz.</p>";

                try
                {
                    await _emailSender.SendAsync(
                        reservation.Customer.Email, 
                        "e-iskele: Rezervasyonunuz Ertelendi", 
                        htmlBody, 
                        smtpSettingsResult.Value, 
                        null, 
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Email send failed: {ex.Message}");
                }
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
