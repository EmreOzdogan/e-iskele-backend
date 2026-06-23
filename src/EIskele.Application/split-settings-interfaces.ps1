$appSettingsDir = "C:\Users\Ozem\Desktop\Projeler\e-iskele Projesi\eiskele\backend\src\EIskele.Application\Common\Settings"
$infraSettingsDir = "C:\Users\Ozem\Desktop\Projeler\e-iskele Projesi\eiskele\backend\src\EIskele.Infrastructure\Settings"

# Create Interfaces
$providerInterface = @"
using System.Threading;
using System.Threading.Tasks;

namespace EIskele.Application.Common.Settings;

public interface ISystemSettingsProvider
{
    Task<string> GetSettingValueAsync(string key, string defaultValue = "", CancellationToken cancellationToken = default);
    Task<T> GetSettingValueAsync<T>(string key, T defaultValue, CancellationToken cancellationToken = default);
    Task<bool> IsFeatureEnabledAsync(string key, bool defaultValue = false, CancellationToken cancellationToken = default);
}
"@
Set-Content -Path "$appSettingsDir\ISystemSettingsProvider.cs" -Value $providerInterface -Encoding UTF8

$generalInterface = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Common.Settings;

public interface IGeneralSettingsService
{
    Task<Result<SystemSettingsDto>> GetGeneralSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateGeneralSettingsAsync(SystemSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);
}
"@
Set-Content -Path "$appSettingsDir\IGeneralSettingsService.cs" -Value $generalInterface -Encoding UTF8

$reservationInterface = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Common.Settings;

public interface IReservationRulesSettingsService
{
    Task<Result<ReservationRulesSettingsDto>> GetReservationRulesSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateReservationRulesSettingsAsync(ReservationRulesSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);
}
"@
Set-Content -Path "$appSettingsDir\IReservationRulesSettingsService.cs" -Value $reservationInterface -Encoding UTF8

$financeInterface = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Common.Settings;

public interface ICommissionFinanceSettingsService
{
    Task<Result<CommissionFinanceSettingsDto>> GetCommissionFinanceSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateCommissionFinanceSettingsAsync(CommissionFinanceSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);
}
"@
Set-Content -Path "$appSettingsDir\ICommissionFinanceSettingsService.cs" -Value $financeInterface -Encoding UTF8

$emailInterface = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Common.Settings;

public interface ISmtpEmailSettingsService
{
    Task<Result<SmtpEmailSettingsDto>> GetSmtpEmailSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateSmtpEmailSettingsAsync(SmtpEmailSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<Result> TestSmtpConnectionAsync(SmtpEmailSettingsDto dto, CancellationToken cancellationToken = default);
    Task<Result> SendTestEmailAsync(string email, SmtpEmailSettingsDto dto, CancellationToken cancellationToken = default);
    Task<Result> SendTestScenarioEmailAsync(string scenarioKey, string email, SmtpEmailSettingsDto dto, CancellationToken cancellationToken = default);
}
"@
Set-Content -Path "$appSettingsDir\ISmtpEmailSettingsService.cs" -Value $emailInterface -Encoding UTF8

$notificationInterface = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Common.Settings;

public interface INotificationSettingsService
{
    Task<Result<NotificationSettingsDto>> GetNotificationSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateNotificationSettingsAsync(NotificationSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);
}
"@
Set-Content -Path "$appSettingsDir\INotificationSettingsService.cs" -Value $notificationInterface -Encoding UTF8

$securityInterface = @"
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
"@
Set-Content -Path "$appSettingsDir\ISecuritySettingsService.cs" -Value $securityInterface -Encoding UTF8

$paymentInterface = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Common.Settings;

public interface IPaymentSettingsService
{
    Task<Result<PaymentSettingsDto>> GetPaymentSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdatePaymentSettingsAsync(PaymentSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);
}
"@
Set-Content -Path "$appSettingsDir\IPaymentSettingsService.cs" -Value $paymentInterface -Encoding UTF8

$smsInterface = @"
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
"@
Set-Content -Path "$appSettingsDir\ISmsSettingsService.cs" -Value $smsInterface -Encoding UTF8

$maintenanceInterface = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Common.Settings;

public interface IMaintenanceModeSettingsService
{
    Task<Result<MaintenanceModeSettingsDto>> GetMaintenanceModeSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateMaintenanceModeSettingsAsync(MaintenanceModeSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);
}
"@
Set-Content -Path "$appSettingsDir\IMaintenanceModeSettingsService.cs" -Value $maintenanceInterface -Encoding UTF8

$auditLogInterface = @"
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Common.Settings;

public interface ISettingsAuditLogService
{
    Task<Result<List<SettingsAuditLogDto>>> GetSettingsAuditLogsAsync(CancellationToken cancellationToken = default);
}
"@
Set-Content -Path "$appSettingsDir\ISettingsAuditLogService.cs" -Value $auditLogInterface -Encoding UTF8
