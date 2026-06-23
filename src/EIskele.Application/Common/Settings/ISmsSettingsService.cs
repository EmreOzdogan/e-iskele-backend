using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Common.Settings;

public interface ISmsSettingsService
{
    Task<Result<SmsSettingsDto>> GetSmsSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateSmsSettingsAsync(SmsSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);
}
