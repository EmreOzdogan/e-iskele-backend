using EIskele.Application.Common.Results;

namespace EIskele.Application.Reviews;

public interface ICaptainReviewService
{
    Task<Result<CaptainReviewsDataDto>> GetReviewsAsync(Guid captainUserId, string? boatId, string? packageId, string? status, string? replyStatus, string? rating, string? reservationId);
    Task<Result<CaptainReviewItemDto>> GetReviewDetailAsync(Guid captainUserId, Guid reviewId);
    Task<Result<bool>> ReplyToReviewAsync(Guid captainUserId, Guid reviewId, ReplyReviewRequestDto request);
    Task<Result<bool>> ReportReviewAsync(Guid captainUserId, Guid reviewId, ReportReviewRequestDto request);
}
