using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Security;
using EIskele.Domain.Entities;
using EIskele.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Security;

public class PermissionService : IPermissionService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly EIskeleDbContext _dbContext;

    public PermissionService(UserManager<ApplicationUser> userManager, EIskeleDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permissionCode, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return false;

        var userRoles = await _userManager.GetRolesAsync(user);
        if (!userRoles.Any())
            return false;

        var hasPermission = await _dbContext.RolePermissions
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .AnyAsync(rp => userRoles.Contains(rp.Role.Name!) && rp.Permission.Code == permissionCode, cancellationToken);

        return hasPermission;
    }
}
