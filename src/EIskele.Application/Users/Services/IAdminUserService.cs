using EIskele.Application.Common.Results;
using EIskele.Application.Users.DTOs;

namespace EIskele.Application.Users.Services;

public interface IAdminUserService
{
    Task<Result<PagedResult<AdminUserListItemDto>>> GetUsersAsync(AdminUserListQuery query, CancellationToken cancellationToken = default);
    Task<Result<AdminUserDetailDto>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> UpdateUserStatusAsync(Guid id, UpdateUserStatusRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<Result> UpdateUserRolesAsync(Guid id, UpdateUserRolesRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeleteUserAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<Result<UserSecurityInfoDto>> GetUserSecurityInfoAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> CreateAdminUserAsync(CreateAdminUserRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
}
