using System.Text.Json.Serialization;
using EIskele.Domain.Enums;

namespace EIskele.Application.Reviews;

public class CaptainReviewItemDto
{
    public Guid Id { get; set; }
    public ReviewCustomerDto Customer { get; set; } = null!;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string CreatedAtText { get; set; } = string.Empty;
    public ReviewBoatDto Boat { get; set; } = null!;
    public ReviewPackageDto Package { get; set; } = null!;
    public ReviewReservationDto Reservation { get; set; } = null!;
    public string Status { get; set; } = string.Empty;
    public string ReplyStatus { get; set; } = string.Empty;
    
    public ReviewCaptainReplyDto? CaptainReply { get; set; }
    
    public string? AdminNote { get; set; }
    public string? RejectionReason { get; set; }
}

public class ReviewCustomerDto
{
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}

public class ReviewBoatDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ReviewPackageDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TourType { get; set; } = string.Empty;
}

public class ReviewReservationDto
{
    public Guid Id { get; set; }
    public string ReservationNo { get; set; } = string.Empty;
    public string TourDateText { get; set; } = string.Empty;
}

public class ReviewCaptainReplyDto
{
    public string Text { get; set; } = string.Empty;
    public string RepliedAtText { get; set; } = string.Empty;
}

public class CaptainReviewsSummaryDto
{
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int UnansweredCount { get; set; }
    public int LowRatingCount { get; set; }
    public int NewThisMonthCount { get; set; }
    public int PublishedCount { get; set; }
}

public class ReviewRatingDistributionDto
{
    public int Rating { get; set; }
    public int Count { get; set; }
}

public class CaptainReviewsDataDto
{
    public CaptainReviewsSummaryDto Summary { get; set; } = new();
    public List<ReviewRatingDistributionDto> RatingDistribution { get; set; } = new();
    public List<CaptainReviewItemDto> Reviews { get; set; } = new();
}

public class ReplyReviewRequestDto
{
    public string ReplyText { get; set; } = string.Empty;
}

public class ReportReviewRequestDto
{
    public string Reason { get; set; } = string.Empty;
    public string? Message { get; set; }
}
