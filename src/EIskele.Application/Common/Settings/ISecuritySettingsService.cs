using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Common.Settings;

public interface ISecuritySettingsService
{
    Task<Result<SecuritySettingsDto>> GetSecuritySettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateSecuritySettingsAsync(SecuritySettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);
}
