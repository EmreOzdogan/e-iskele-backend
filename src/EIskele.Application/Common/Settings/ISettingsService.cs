using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Common.Settings;

public interface ISettingsService
{
    Task<string> GetSettingValueAsync(string key, string defaultValue = "", CancellationToken cancellationToken = default);
    Task<T> GetSettingValueAsync<T>(string key, T defaultValue, CancellationToken cancellationToken = default);
    Task<bool> IsFeatureEnabledAsync(string key, bool defaultValue = false, CancellationToken cancellationToken = default);
    Task<Result<SystemSettingsDto>> GetGeneralSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateGeneralSettingsAsync(SystemSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<Result<ReservationRulesSettingsDto>> GetReservationRulesSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateReservationRulesSettingsAsync(ReservationRulesSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);

    Task<Result<CommissionFinanceSettingsDto>> GetCommissionFinanceSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateCommissionFinanceSettingsAsync(CommissionFinanceSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);

    Task<Result<SmtpEmailSettingsDto>> GetSmtpEmailSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateSmtpEmailSettingsAsync(SmtpEmailSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);

    Task<Result> TestSmtpConnectionAsync(SmtpEmailSettingsDto dto, CancellationToken cancellationToken = default);
    Task<Result> SendTestEmailAsync(string email, SmtpEmailSettingsDto dto, CancellationToken cancellationToken = default);

    Task<Result<NotificationSettingsDto>> GetNotificationSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateNotificationSettingsAsync(NotificationSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);
}
