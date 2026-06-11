using EIskele.Application.Common.Errors;
using EIskele.Application.Common.Results;
using EIskele.Application.Users.DTOs;
using EIskele.Application.Users.Services;
using EIskele.Domain.Entities;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Services;

public class AdminUserService : IAdminUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly EIskeleDbContext _dbContext;
    
    public AdminUserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        EIskeleDbContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<AdminUserListItemDto>>> GetUsersAsync(AdminUserListQuery query, CancellationToken cancellationToken = default)
    {
        var usersQuery = _userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            usersQuery = usersQuery.Where(x => 
                x.FirstName.ToLower().Contains(search) || 
                x.LastName.ToLower().Contains(search) || 
                x.Email!.ToLower().Contains(search));
        }

        if (query.Status.HasValue)
        {
            usersQuery = usersQuery.Where(x => x.Status == query.Status.Value);
        }

        if (query.CreatedDateStart.HasValue)
        {
            usersQuery = usersQuery.Where(x => x.CreatedAt >= query.CreatedDateStart.Value);
        }

        if (query.CreatedDateEnd.HasValue)
        {
            usersQuery = usersQuery.Where(x => x.CreatedAt <= query.CreatedDateEnd.Value);
        }
        
        // Sorting
        usersQuery = query.SortDirection?.ToLower() == "desc" 
            ? usersQuery.OrderByDescending(x => x.CreatedAt) 
            : usersQuery.OrderBy(x => x.CreatedAt);

        var totalCount = await usersQuery.CountAsync(cancellationToken);
        
        var users = await usersQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        // Map to DTO
        var items = new List<AdminUserListItemDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            // To filter by role properly, we might need a more complex join, but for MVP we can filter in memory or do it via UserRoles.
            if (!string.IsNullOrWhiteSpace(query.Role) && !roles.Contains(query.Role, StringComparer.OrdinalIgnoreCase))
            {
                totalCount--;
                continue;
            }

            items.Add(new AdminUserListItemDto
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email ?? string.Empty,
                Phone = user.PhoneNumber,
                Status = user.Status,
                Roles = roles.ToList(),
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            });
        }

        return Result<PagedResult<AdminUserListItemDto>>.Success(new PagedResult<AdminUserListItemDto>(items, totalCount, query.Page, query.PageSize));
    }

    public async Task<Result<AdminUserDetailDto>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return Result<AdminUserDetailDto>.Failure(new Error("UserNotFound", "Kullanıcı bulunamadı."));
        }

        var roles = await _userManager.GetRolesAsync(user);

        var dto = new AdminUserDetailDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            Phone = user.PhoneNumber,
            ProfileImageUrl = user.ProfileImageUrl,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            Status = user.Status,
            Roles = roles.ToList(),
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };

        return Result<AdminUserDetailDto>.Success(dto);
    }

    public async Task<Result> UpdateUserStatusAsync(Guid id, UpdateUserStatusRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            return Result.Failure(new Error("UserNotFound", "Kullanıcı bulunamadı."));

        var isTargetSuperAdmin = await _userManager.IsInRoleAsync(user, "SuperAdmin");
        
        var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
        var isCurrentSuperAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "SuperAdmin");

        // SuperAdmin cannot be deactivated by a normal Admin
        if (isTargetSuperAdmin && !isCurrentSuperAdmin && request.Status != UserStatus.Active)
        {
            return Result.Failure(new Error("Forbidden", "SuperAdmin kullanıcısı normal Admin tarafından pasife alınamaz."));
        }

        // The last active SuperAdmin cannot be deactivated
        if (isTargetSuperAdmin && request.Status != UserStatus.Active)
        {
            var superAdmins = await _userManager.GetUsersInRoleAsync("SuperAdmin");
            var activeSuperAdminsCount = superAdmins.Count(x => x.Status == UserStatus.Active && !x.IsDeleted);
            
            if (activeSuperAdminsCount <= 1 && user.Status == UserStatus.Active)
            {
                return Result.Failure(new Error("Forbidden", "Sistemde kalan son aktif SuperAdmin pasife alınamaz."));
            }
        }

        var oldStatus = user.Status;
        user.Status = request.Status;
        
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Result.Failure(new Error("UpdateFailed", "Kullanıcı durumu güncellenemedi."));

        // Add AuditLog
        await CreateAuditLogAsync("UpdateStatus", "ApplicationUser", user.Id.ToString(), currentUserId, oldStatus.ToString(), request.Status.ToString(), request.Reason ?? "Status updated", cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UpdateUserRolesAsync(Guid id, UpdateUserRolesRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            return Result.Failure(new Error("UserNotFound", "Kullanıcı bulunamadı."));

        var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
        var isCurrentSuperAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "SuperAdmin");

        var currentRoles = await _userManager.GetRolesAsync(user);

        // Validate roles
        foreach (var role in request.Roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                return Result.Failure(new Error("InvalidRole", $"Geçersiz rol: {role}"));
        }

        // SuperAdmin role logic
        var addingSuperAdmin = request.Roles.Contains("SuperAdmin", StringComparer.OrdinalIgnoreCase) && !currentRoles.Contains("SuperAdmin", StringComparer.OrdinalIgnoreCase);
        var removingSuperAdmin = !request.Roles.Contains("SuperAdmin", StringComparer.OrdinalIgnoreCase) && currentRoles.Contains("SuperAdmin", StringComparer.OrdinalIgnoreCase);

        if ((addingSuperAdmin || removingSuperAdmin) && !isCurrentSuperAdmin)
        {
            return Result.Failure(new Error("Forbidden", "SuperAdmin yetkisi sadece başka bir SuperAdmin tarafından atanabilir veya kaldırılabilir."));
        }

        if (removingSuperAdmin)
        {
            var superAdmins = await _userManager.GetUsersInRoleAsync("SuperAdmin");
            var activeSuperAdminsCount = superAdmins.Count(x => x.Status == UserStatus.Active && !x.IsDeleted);
            if (activeSuperAdminsCount <= 1)
            {
                return Result.Failure(new Error("Forbidden", "Sistemde kalan son aktif SuperAdmin rolü kaldırılamaz."));
            }
        }

        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded) return Result.Failure(new Error("UpdateRolesFailed", "Mevcut roller kaldırılamadı."));

        var addResult = await _userManager.AddToRolesAsync(user, request.Roles);
        if (!addResult.Succeeded) return Result.Failure(new Error("UpdateRolesFailed", "Yeni roller atanamadı."));

        // Add AuditLog
        await CreateAuditLogAsync("UpdateRoles", "ApplicationUser", user.Id.ToString(), currentUserId, string.Join(",", currentRoles), string.Join(",", request.Roles), "Roles updated", cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteUserAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            return Result.Failure(new Error("UserNotFound", "Kullanıcı bulunamadı."));

        var isSuperAdmin = await _userManager.IsInRoleAsync(user, "SuperAdmin");
        
        if (isSuperAdmin)
        {
            var superAdmins = await _userManager.GetUsersInRoleAsync("SuperAdmin");
            var activeSuperAdminsCount = superAdmins.Count(x => !x.IsDeleted);
            if (activeSuperAdminsCount <= 1)
            {
                return Result.Failure(new Error("Forbidden", "Sistemde kalan son SuperAdmin silinemez."));
            }
        }

        user.IsDeleted = true;
        user.Status = UserStatus.Deleted;
        user.DeletedAt = DateTime.UtcNow;
        user.DeletedBy = currentUserId;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) return Result.Failure(new Error("DeleteFailed", "Kullanıcı silinemedi."));

        // Add AuditLog
        await CreateAuditLogAsync("SoftDelete", "ApplicationUser", user.Id.ToString(), currentUserId, "Active", "Deleted", "User soft deleted", cancellationToken);

        return Result.Success();
    }

    public async Task<Result<UserSecurityInfoDto>> GetUserSecurityInfoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return Result<UserSecurityInfoDto>.Failure(new Error("UserNotFound", "Kullanıcı bulunamadı."));
        }

        var isLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow;

        var dto = new UserSecurityInfoDto
        {
            EmailVerified = user.EmailConfirmed,
            PhoneVerified = user.PhoneNumberConfirmed,
            TwoFactorEnabled = user.TwoFactorEnabled,
            LastLoginAt = user.LastLoginAt,
            LastLoginIp = user.LastLoginIp,
            FailedLoginAttempts = user.AccessFailedCount,
            IsLocked = isLocked,
            LastPasswordChangedAt = user.LastPasswordChangedAt,
            ActiveSessionCount = user.ActiveSessionCount
        };

        return Result<UserSecurityInfoDto>.Success(dto);
    }

    public async Task<Result> CreateAdminUserAsync(CreateAdminUserRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
        var isCurrentSuperAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "SuperAdmin");

        var roleName = string.Equals(request.Role, "superadmin", StringComparison.OrdinalIgnoreCase) ? "SuperAdmin" : "Admin";

        if (roleName == "SuperAdmin" && !isCurrentSuperAdmin)
        {
            return Result.Failure(new Error("Forbidden", "Sadece SuperAdmin yetkisine sahip kullanıcılar yeni SuperAdmin oluşturabilir."));
        }

        if (await _userManager.FindByEmailAsync(request.Email) != null)
        {
            return Result.Failure(new Error("Conflict", "Bu e-posta adresi sistemde zaten kayıtlı."));
        }

        var names = request.FullName.Trim().Split(' ');
        var firstName = names.Length > 0 ? names[0] : "";
        var lastName = names.Length > 1 ? string.Join(" ", names.Skip(1)) : "";

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = request.Phone,
            Status = UserStatus.Active,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId
        };

        var password = request.GenerateTempPassword ? "Eiskele123!" : "Eiskele123!"; 

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            return Result.Failure(new Error("CreateFailed", "Kullanıcı oluşturulamadı. " + string.Join(", ", createResult.Errors.Select(e => e.Description))));
        }

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
        }

        var roleResult = await _userManager.AddToRoleAsync(user, roleName);
        if (!roleResult.Succeeded)
        {
            return Result.Failure(new Error("AssignRoleFailed", "Kullanıcıya rol atanamadı."));
        }

        await CreateAuditLogAsync("CreateAdmin", "ApplicationUser", user.Id.ToString(), currentUserId, "", roleName, $"Created new {roleName}", cancellationToken);

        return Result.Success();
    }

    private async Task CreateAuditLogAsync(string action, string entityType, string entityId, Guid actorUserId, string oldValue, string newValue, string description, CancellationToken cancellationToken)
    {
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            ActorUserId = actorUserId,
            OldValue = oldValue,
            NewValue = newValue,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Set<AuditLog>().AddAsync(auditLog, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
