using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Boats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers;

[Route("api/[controller]")]
public class BoatsController : BaseController
{
    private readonly IBoatService _boatService;

    public BoatsController(IBoatService boatService)
    {
        _boatService = boatService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBoat([FromBody] CreateBoatRequest request, CancellationToken cancellationToken)
    {
        var result = await _boatService.CreateBoatAsync(request, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/submit-for-review")]
    public async Task<IActionResult> SubmitForReview(Guid id, CancellationToken cancellationToken)
    {
        var result = await _boatService.SubmitForReviewAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> ApproveBoat(Guid id, CancellationToken cancellationToken)
    {
        var result = await _boatService.ApproveBoatAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/tour-packages")]
    public async Task<IActionResult> AddTourPackage(Guid id, [FromBody] CreateTourPackageRequest request, CancellationToken cancellationToken)
    {
        if (id != request.BoatId)
        {
            return BadRequest("Route ID ile Body ID eşleşmiyor.");
        }

        var result = await _boatService.AddTourPackageAsync(request, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("my-boats")]
    [Authorize(Roles = "Captain")]
    public async Task<IActionResult> GetMyBoats(CancellationToken cancellationToken)
    {
        var result = await _boatService.GetMyBoatsAsync(UserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Captain")]
    public async Task<IActionResult> GetMyBoatDetail(Guid id, CancellationToken cancellationToken)
    {
        var result = await _boatService.GetMyBoatDetailAsync(id, UserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Captain")]
    public async Task<IActionResult> UpdateMyBoat(Guid id, [FromBody] UpdateCaptainBoatRequest request, CancellationToken cancellationToken)
    {
        var result = await _boatService.UpdateMyBoatAsync(id, UserId, request, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("my-boats")]
    [Authorize(Roles = "Captain")]
    public async Task<IActionResult> CreateMyBoat([FromBody] CreateCaptainBoatRequest request, CancellationToken cancellationToken)
    {
        var result = await _boatService.CreateMyBoatAsync(UserId, request, cancellationToken);
        return HandleResult(result);
    }
}
