using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Layout;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Services;

public class CaptainLayoutService : ICaptainLayoutService
{
    private readonly EIskeleDbContext _context;

    public CaptainLayoutService(EIskeleDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CaptainLayoutDataDto>> GetLayoutDataAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Guid.TryParse(userId, out var userGuid))
                return Result<CaptainLayoutDataDto>.Failure(new EIskele.Application.Common.Errors.Error("User.InvalidId", "Geçersiz kullanıcı ID'si."));

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userGuid, cancellationToken);
            if (user == null)
                return Result<CaptainLayoutDataDto>.Failure(new EIskele.Application.Common.Errors.Error("User.NotFound", "Kullanıcı bulunamadı."));

            var captain = await _context.Captains
                .Include(c => c.Company)
                .FirstOrDefaultAsync(c => c.UserId == userGuid, cancellationToken);

            var boats = new List<EIskele.Domain.Entities.Boat>();
            var reservations = new List<EIskele.Domain.Entities.Reservation>();
            var reviews = new List<EIskele.Domain.Entities.Review>();
            
            if (captain != null)
            {
                boats = await _context.Boats.Where(b => b.CaptainId == captain.Id && !b.IsDeleted).ToListAsync(cancellationToken);
                reservations = await _context.Reservations.Where(r => r.Boat.CaptainId == captain.Id).ToListAsync(cancellationToken);
                reviews = await _context.Reviews.Where(r => r.Reservation.Boat.CaptainId == captain.Id).ToListAsync(cancellationToken);
            }

            var primaryBoat = boats.FirstOrDefault();

            // Populate Profile
            var profile = new CaptainProfileLayoutDto
            {
                FullName = $"{user.FirstName} {user.LastName}",
                DisplayName = $"{user.FirstName} {user.LastName}",
                AccountType = captain?.Company != null ? "Kurumsal kaptan" : "Bireysel kaptan",
                VerificationStatus = captain?.Status == "Approved" ? "Onaylı" : "İncelemede",
                AvatarUrl = "",
                CompanyName = captain?.Company?.CompanyName ?? "",
                PrimaryBoatName = primaryBoat?.Name ?? "Tekne Eklenmedi",
                AccountStatus = captain == null ? "pendingReview" : (captain.Status == "Approved" ? "active" : "missingDocuments")
            };

            // Populate Counters
            var pendingResCount = reservations.Count(r => r.Status == ReservationStatus.WaitingCaptainApproval);
            var unansweredRevCount = reviews.Count(r => r.Reply == null);
            // Mocking document actions and support for now until those entities are fully modeled
            var docActions = captain != null && captain.Status != "Approved" ? 1 : 0;

            var counters = new CaptainCountersDto
            {
                PendingReservations = pendingResCount,
                DocumentActions = docActions,
                UnansweredReviews = unansweredRevCount,
                SupportWaitingCaptain = 0,
                UnreadNotifications = 0
            };

            // Populate Notifications
            var dbNotifications = await _context.Notifications
                .Where(n => n.UserId == userGuid)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToListAsync(cancellationToken);

            var notifications = dbNotifications.Select(n => new CaptainNotificationDto
            {
                Id = n.Id.ToString(),
                Title = n.Subject,
                Description = n.Body,
                Type = n.Type, // Ensure it aligns with "reservation", "document", "review"
                CreatedAtText = n.CreatedAt.ToString("dd MMM HH:mm"),
                ActionPath = "/panel", // Can be extended to be dynamic based on type
                Read = n.Status == "Read"
            }).ToList();

            counters.UnreadNotifications = await _context.Notifications.CountAsync(n => n.UserId == userGuid && n.Status != "Read", cancellationToken);

            return Result<CaptainLayoutDataDto>.Success(new CaptainLayoutDataDto
            {
                Captain = profile,
                Counters = counters,
                Notifications = notifications
            });
        }
        catch (Exception ex)
        {
            return Result<CaptainLayoutDataDto>.Failure(new EIskele.Application.Common.Errors.Error("ServerError", ex.ToString()));
        }
    }
}
