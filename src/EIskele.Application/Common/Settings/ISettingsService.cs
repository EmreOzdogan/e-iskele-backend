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
    Task<Result> SendTestScenarioEmailAsync(string scenarioKey, string email, SmtpEmailSettingsDto dto, CancellationToken cancellationToken = default);

    Task<Result<NotificationSettingsDto>> GetNotificationSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateNotificationSettingsAsync(NotificationSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);

    Task<Result<SecuritySettingsDto>> GetSecuritySettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateSecuritySettingsAsync(SecuritySettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);

    Task<Result<PaymentSettingsDto>> GetPaymentSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdatePaymentSettingsAsync(PaymentSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);

    Task<Result<SmsSettingsDto>> GetSmsSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateSmsSettingsAsync(SmsSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);

    Task<Result<MaintenanceModeSettingsDto>> GetMaintenanceModeSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateMaintenanceModeSettingsAsync(MaintenanceModeSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);

    Task<Result<System.Collections.Generic.List<SettingsAuditLogDto>>> GetSettingsAuditLogsAsync(CancellationToken cancellationToken = default);
}
