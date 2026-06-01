using System;
using System.Threading;
using System.Threading.Tasks;

namespace EIskele.Application.Common.Security;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(Guid userId, string permissionCode, CancellationToken cancellationToken = default);
}
