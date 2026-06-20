using EIskele.Application.Common.Results;
using EIskele.Application.Settings.DTOs;

namespace EIskele.Application.Settings;

public interface ICaptainSettingsService
{
    Task<Result<CaptainSettingsDto>> GetSettingsAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<bool>> UpdateProfileAsync(Guid userId, UpdateCaptainProfileDto request, CancellationToken cancellationToken);
    Task<Result<bool>> UpdatePaymentAsync(Guid userId, UpdateCaptainPaymentDto request, CancellationToken cancellationToken);
    Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangeCaptainPasswordDto request, CancellationToken cancellationToken);
    Task<Result<bool>> RevokeOtherSessionsAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<bool>> SaveNotificationPreferencesAsync(Guid userId, UpdateNotificationPreferencesDto request, CancellationToken cancellationToken);
}
