using EIskele.Application.Common.Results;
using EIskele.Application.Reviews;
using EIskele.Domain.Entities;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Services;

public class CaptainReviewService : ICaptainReviewService
{
    private readonly EIskeleDbContext _dbContext;

    public CaptainReviewService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<CaptainReviewsDataDto>> GetReviewsAsync(Guid captainUserId, string? boatId, string? packageId, string? status, string? replyStatus, string? rating, string? reservationId)
    {
        var captain = await _dbContext.Captains.FirstOrDefaultAsync(c => c.UserId == captainUserId);
        if (captain == null) return Result<CaptainReviewsDataDto>.Failure("Kaptan bulunamadı", "CAPTAIN_NOT_FOUND");

        var query = _dbContext.Reviews
            .Include(r => r.Customer)
            .Include(r => r.Boat)
            .Include(r => r.TourPackage)
            .Include(r => r.Reservation)
            .Include(r => r.Reply)
            .Where(r => r.Boat.CaptainId == captain.Id);

        if (!string.IsNullOrEmpty(boatId) && boatId != "all" && Guid.TryParse(boatId, out var bId))
        {
            query = query.Where(r => r.BoatId == bId);
        }

        if (!string.IsNullOrEmpty(packageId) && packageId != "all" && Guid.TryParse(packageId, out var pId))
        {
            query = query.Where(r => r.TourPackageId == pId);
        }
        
        if (!string.IsNullOrEmpty(reservationId) && Guid.TryParse(reservationId, out var resId))
        {
            query = query.Where(r => r.ReservationId == resId);
        }

        var reviewsList = await query.ToListAsync();

        var dtoList = new List<CaptainReviewItemDto>();
        foreach (var r in reviewsList)
        {
            dtoList.Add(new CaptainReviewItemDto
            {
                Id = r.Id,
                Customer = new ReviewCustomerDto { FullName = r.Customer.FirstName + " " + r.Customer.LastName, AvatarUrl = r.Customer.ProfileImageUrl },
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAtText = r.CreatedAt.ToString("dd MMM yyyy"),
                Boat = new ReviewBoatDto { Id = r.BoatId, Name = r.Boat.Name },
                Package = new ReviewPackageDto { Id = r.TourPackageId, Name = r.TourPackage.Name, TourType = r.TourPackage.TourType.ToString() },
                Reservation = new ReviewReservationDto { Id = r.ReservationId, ReservationNo = r.Reservation?.ReservationNo ?? "", TourDateText = r.Reservation?.StartDateTime.ToString("dd MMM yyyy HH:mm") ?? "" },
                Status = r.Status.ToString(),
                ReplyStatus = r.Reply != null ? "answered" : "unanswered",
                CaptainReply = r.Reply != null ? new ReviewCaptainReplyDto { Text = r.Reply.ReplyText, RepliedAtText = r.Reply.CreatedAt.ToString("dd MMM yyyy HH:mm") } : null
            });
        }

        if (!string.IsNullOrEmpty(status) && status != "all")
        {
            dtoList = dtoList.Where(x => string.Equals(x.Status, status, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrEmpty(replyStatus) && replyStatus != "all")
        {
            dtoList = dtoList.Where(x => string.Equals(x.ReplyStatus, replyStatus, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrEmpty(rating) && rating != "all")
        {
            if (rating == "low")
                dtoList = dtoList.Where(x => x.Rating <= 3).ToList();
            else if (int.TryParse(rating, out var rVal))
                dtoList = dtoList.Where(x => x.Rating == rVal).ToList();
        }

        var summary = new CaptainReviewsSummaryDto
        {
            TotalReviews = dtoList.Count,
            AverageRating = dtoList.Any() ? dtoList.Average(x => x.Rating) : 0,
            UnansweredCount = dtoList.Count(x => x.ReplyStatus == "unanswered"),
            LowRatingCount = dtoList.Count(x => x.Rating <= 3),
            PublishedCount = dtoList.Count(x => x.Status == "Published")
        };

        var dist = new List<ReviewRatingDistributionDto>();
        for (int i = 5; i >= 1; i--)
        {
            dist.Add(new ReviewRatingDistributionDto { Rating = i, Count = dtoList.Count(x => x.Rating == i) });
        }

        var result = new CaptainReviewsDataDto
        {
            Summary = summary,
            RatingDistribution = dist,
            Reviews = dtoList
        };

        return Result<CaptainReviewsDataDto>.Success(result);
    }

    public async Task<Result<CaptainReviewItemDto>> GetReviewDetailAsync(Guid captainUserId, Guid reviewId)
    {
        var captain = await _dbContext.Captains.FirstOrDefaultAsync(c => c.UserId == captainUserId);
        if (captain == null) return Result<CaptainReviewItemDto>.Failure("Kaptan bulunamadı", "CAPTAIN_NOT_FOUND");

        var r = await _dbContext.Reviews
            .Include(x => x.Customer)
            .Include(x => x.Boat)
            .Include(x => x.TourPackage)
            .Include(x => x.Reservation)
            .Include(x => x.Reply)
            .FirstOrDefaultAsync(x => x.Id == reviewId && x.Boat.CaptainId == captain.Id);

        if (r == null) return Result<CaptainReviewItemDto>.Failure("Yorum bulunamadı veya yetkiniz yok", "NOT_FOUND");

        var dto = new CaptainReviewItemDto
        {
            Id = r.Id,
            Customer = new ReviewCustomerDto { FullName = r.Customer.FirstName + " " + r.Customer.LastName, AvatarUrl = r.Customer.ProfileImageUrl },
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAtText = r.CreatedAt.ToString("dd MMM yyyy"),
            Boat = new ReviewBoatDto { Id = r.BoatId, Name = r.Boat.Name },
            Package = new ReviewPackageDto { Id = r.TourPackageId, Name = r.TourPackage.Name, TourType = r.TourPackage.TourType.ToString() },
            Reservation = new ReviewReservationDto { Id = r.ReservationId, ReservationNo = r.Reservation?.ReservationNo ?? "", TourDateText = r.Reservation?.StartDateTime.ToString("dd MMM yyyy HH:mm") ?? "" },
            Status = r.Status.ToString(),
            ReplyStatus = r.Reply != null ? "answered" : "unanswered",
            CaptainReply = r.Reply != null ? new ReviewCaptainReplyDto { Text = r.Reply.ReplyText, RepliedAtText = r.Reply.CreatedAt.ToString("dd MMM yyyy HH:mm") } : null
        };

        return Result<CaptainReviewItemDto>.Success(dto);
    }

    public async Task<Result<bool>> ReplyToReviewAsync(Guid captainUserId, Guid reviewId, ReplyReviewRequestDto request)
    {
        var captain = await _dbContext.Captains.FirstOrDefaultAsync(c => c.UserId == captainUserId);
        if (captain == null) return Result<bool>.Failure("Kaptan bulunamadı", "CAPTAIN_NOT_FOUND");

        var review = await _dbContext.Reviews
            .Include(x => x.Boat)
            .Include(x => x.Reply)
            .FirstOrDefaultAsync(x => x.Id == reviewId && x.Boat.CaptainId == captain.Id);

        if (review == null) return Result<bool>.Failure("Yorum bulunamadı veya yetkiniz yok", "NOT_FOUND");
        if (string.IsNullOrWhiteSpace(request.ReplyText)) return Result<bool>.Failure("Cevap boş olamaz", "VALIDATION_ERROR");

        if (review.Reply == null)
        {
            review.Reply = new ReviewReply
            {
                ReviewId = review.Id,
                ReplyText = request.ReplyText
            };
        }
        else
        {
            review.Reply.ReplyText = request.ReplyText;
        }

        await _dbContext.SaveChangesAsync();
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ReportReviewAsync(Guid captainUserId, Guid reviewId, ReportReviewRequestDto request)
    {
        var captain = await _dbContext.Captains.FirstOrDefaultAsync(c => c.UserId == captainUserId);
        if (captain == null) return Result<bool>.Failure("Kaptan bulunamadı", "CAPTAIN_NOT_FOUND");

        var review = await _dbContext.Reviews
            .Include(x => x.Boat)
            .FirstOrDefaultAsync(x => x.Id == reviewId && x.Boat.CaptainId == captain.Id);

        if (review == null) return Result<bool>.Failure("Yorum bulunamadı veya yetkiniz yok", "NOT_FOUND");
        
        var report = new ReviewReport
        {
            ReviewId = review.Id,
            Reason = request.Reason,
            Message = request.Message,
            IsResolved = false
        };

        _dbContext.ReviewReports.Add(report);
        review.Status = Domain.Enums.ReviewStatus.InReview; // Mark as reported basically or create another status.

        await _dbContext.SaveChangesAsync();
        return Result<bool>.Success(true);
    }
}
