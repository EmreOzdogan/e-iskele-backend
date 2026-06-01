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
}
