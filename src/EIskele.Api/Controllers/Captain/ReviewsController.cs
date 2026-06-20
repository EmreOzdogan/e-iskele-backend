using EIskele.Application.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers.Captain;

[Authorize(Roles = "Captain")]
[Route("api/captain/reviews")]
public class ReviewsController : BaseController
{
    private readonly ICaptainReviewService _reviewService;

    public ReviewsController(ICaptainReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<IActionResult> GetReviews([FromQuery] string? boatId, [FromQuery] string? packageId, [FromQuery] string? status, [FromQuery] string? replyStatus, [FromQuery] string? rating, [FromQuery] string? reservationId)
    {
        var result = await _reviewService.GetReviewsAsync(UserId, boatId, packageId, status, replyStatus, rating, reservationId);
        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReviewDetail(Guid id)
    {
        var result = await _reviewService.GetReviewDetailAsync(UserId, id);
        return HandleResult(result);
    }

    [HttpPost("{id}/reply")]
    public async Task<IActionResult> ReplyToReview(Guid id, [FromBody] ReplyReviewRequestDto request)
    {
        var result = await _reviewService.ReplyToReviewAsync(UserId, id, request);
        return HandleResult(result);
    }

    [HttpPost("{id}/report")]
    public async Task<IActionResult> ReportReview(Guid id, [FromBody] ReportReviewRequestDto request)
    {
        var result = await _reviewService.ReportReviewAsync(UserId, id, request);
        return HandleResult(result);
    }
}
