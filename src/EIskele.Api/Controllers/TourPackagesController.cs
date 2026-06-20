using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Packages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers;

[Route("api/tour-packages")]
public class TourPackagesController : BaseController
{
    private readonly ITourPackageService _packageService;

    public TourPackagesController(ITourPackageService packageService)
    {
        _packageService = packageService;
    }

    [HttpGet("my-packages")]
    [Authorize(Roles = "Captain")]
    public async Task<IActionResult> GetMyPackages(CancellationToken cancellationToken)
    {
        var result = await _packageService.GetMyPackagesAsync(UserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Captain")]
    public async Task<IActionResult> GetMyPackageDetail(Guid id, CancellationToken cancellationToken)
    {
        var result = await _packageService.GetMyPackageDetailAsync(id, UserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Captain")]
    public async Task<IActionResult> CreateMyPackage([FromBody] CreateCaptainPackageRequest request, CancellationToken cancellationToken)
    {
        var result = await _packageService.CreateMyPackageAsync(UserId, request, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Captain")]
    public async Task<IActionResult> UpdateMyPackage(Guid id, [FromBody] UpdateCaptainPackageRequest request, CancellationToken cancellationToken)
    {
        var result = await _packageService.UpdateMyPackageAsync(id, UserId, request, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/activate")]
    [Authorize(Roles = "Captain")]
    public async Task<IActionResult> ActivateMyPackage(Guid id, CancellationToken cancellationToken)
    {
        var result = await _packageService.ActivateMyPackageAsync(id, UserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Roles = "Captain")]
    public async Task<IActionResult> DeactivateMyPackage(Guid id, CancellationToken cancellationToken)
    {
        var result = await _packageService.DeactivateMyPackageAsync(id, UserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/duplicate")]
    [Authorize(Roles = "Captain")]
    public async Task<IActionResult> DuplicateMyPackage(Guid id, CancellationToken cancellationToken)
    {
        var result = await _packageService.DuplicateMyPackageAsync(id, UserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Captain")]
    public async Task<IActionResult> DeleteMyPackage(Guid id, CancellationToken cancellationToken)
    {
        var result = await _packageService.DeleteMyPackageAsync(id, UserId, cancellationToken);
        return HandleResult(result);
    }
}
