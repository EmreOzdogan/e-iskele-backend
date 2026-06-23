using EIskele.Application.Users.DTOs;
using EIskele.Application.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EIskele.Api.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminUsersController : BaseController
{
    private readonly IAdminUserService _adminUserService;

    public AdminUsersController(IAdminUserService adminUserService)
    {
        _adminUserService = adminUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] AdminUserListQuery query, CancellationToken cancellationToken)
    {
        var result = await _adminUserService.GetUsersAsync(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _adminUserService.GetUserByIdAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}/security")]
    public async Task<IActionResult> GetUserSecurityInfo(Guid id, CancellationToken cancellationToken)
    {
        var result = await _adminUserService.GetUserSecurityInfoAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateUserStatus(Guid id, [FromBody] UpdateUserStatusRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = this.UserId;
        var result = await _adminUserService.UpdateUserStatusAsync(id, request, currentUserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}/roles")]
    public async Task<IActionResult> UpdateUserRoles(Guid id, [FromBody] UpdateUserRolesRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = this.UserId;
        var result = await _adminUserService.UpdateUserRolesAsync(id, request, currentUserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAdminUser([FromBody] CreateAdminUserRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = this.UserId;
        var result = await _adminUserService.CreateAdminUserAsync(request, currentUserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var currentUserId = this.UserId;
        var result = await _adminUserService.DeleteUserAsync(id, currentUserId, cancellationToken);
        return HandleResult(result);
    }
}
