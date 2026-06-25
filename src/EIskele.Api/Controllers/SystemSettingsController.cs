using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers;

[ApiController]
[Route("api/admin/settings")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class SystemSettingsController : BaseController
{
    private readonly IGeneralSettingsService _generalSettingsService;
    private readonly IReservationRulesSettingsService _reservationRulesSettingsService;
    private readonly ICommissionFinanceSettingsService _commissionFinanceSettingsService;
    private readonly ISmtpEmailSettingsService _smtpEmailSettingsService;
    private readonly INotificationSettingsService _notificationSettingsService;
    private readonly ISecuritySettingsService _securitySettingsService;
    private readonly IPaymentSettingsService _paymentSettingsService;
    private readonly ISmsSettingsService _smsSettingsService;
    private readonly IMaintenanceModeSettingsService _maintenanceModeSettingsService;
    private readonly ISettingsAuditLogService _settingsAuditLogService;

    public SystemSettingsController(
        IGeneralSettingsService generalSettingsService,
        IReservationRulesSettingsService reservationRulesSettingsService,
        ICommissionFinanceSettingsService commissionFinanceSettingsService,
        ISmtpEmailSettingsService smtpEmailSettingsService,
        INotificationSettingsService notificationSettingsService,
        ISecuritySettingsService securitySettingsService,
        IPaymentSettingsService paymentSettingsService,
        ISmsSettingsService smsSettingsService,
        IMaintenanceModeSettingsService maintenanceModeSettingsService,
        ISettingsAuditLogService settingsAuditLogService)
    {
        _generalSettingsService = generalSettingsService;
        _reservationRulesSettingsService = reservationRulesSettingsService;
        _commissionFinanceSettingsService = commissionFinanceSettingsService;
        _smtpEmailSettingsService = smtpEmailSettingsService;
        _notificationSettingsService = notificationSettingsService;
        _securitySettingsService = securitySettingsService;
        _paymentSettingsService = paymentSettingsService;
        _smsSettingsService = smsSettingsService;
        _maintenanceModeSettingsService = maintenanceModeSettingsService;
        _settingsAuditLogService = settingsAuditLogService;
    }

    [HttpGet("general")]
    public async Task<IActionResult> GetGeneralSettings(CancellationToken cancellationToken)
    {
        var result = await _generalSettingsService.GetGeneralSettingsAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("general")]
    public async Task<IActionResult> UpdateGeneralSettings([FromBody] SystemSettingsDto dto, CancellationToken cancellationToken)
    {
        var result = await _generalSettingsService.UpdateGeneralSettingsAsync(dto, this.UserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("reservation-rules")]
    public async Task<IActionResult> GetReservationRules(CancellationToken cancellationToken)
    {
        var result = await _reservationRulesSettingsService.GetReservationRulesSettingsAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("reservation-rules")]
    public async Task<IActionResult> UpdateReservationRules([FromBody] ReservationRulesSettingsDto dto, CancellationToken cancellationToken)
    {
        var result = await _reservationRulesSettingsService.UpdateReservationRulesSettingsAsync(dto, this.UserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("commission-finance")]
    public async Task<IActionResult> GetCommissionFinance(CancellationToken cancellationToken)
    {
        var result = await _commissionFinanceSettingsService.GetCommissionFinanceSettingsAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("commission-finance")]
    public async Task<IActionResult> UpdateCommissionFinance([FromBody] CommissionFinanceSettingsDto dto, CancellationToken cancellationToken)
    {
        var result = await _commissionFinanceSettingsService.UpdateCommissionFinanceSettingsAsync(dto, this.UserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("smtp-email")]
    public async Task<IActionResult> GetSmtpEmail(CancellationToken cancellationToken)
    {
        var result = await _smtpEmailSettingsService.GetSmtpEmailSettingsAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("smtp-email")]
    public async Task<IActionResult> UpdateSmtpEmail([FromBody] SmtpEmailSettingsDto dto, CancellationToken cancellationToken)
    {
        var result = await _smtpEmailSettingsService.UpdateSmtpEmailSettingsAsync(dto, this.UserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("smtp-email/test")]
    public async Task<IActionResult> TestSmtpEmail([FromBody] SmtpEmailSettingsDto dto, CancellationToken cancellationToken)
    {
        var result = await _smtpEmailSettingsService.TestSmtpConnectionAsync(dto, cancellationToken);
        return HandleResult(result, "SMTP bağlantısı başarılı.");
    }

    public class SendTestEmailRequest
    {
        public string Email { get; set; } = string.Empty;
        public SmtpEmailSettingsDto Settings { get; set; } = new();
    }

    [HttpPost("smtp-email/send-test")]
    public async Task<IActionResult> SendTestEmail([FromBody] SendTestEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await _smtpEmailSettingsService.SendTestEmailAsync(request.Email, request.Settings, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("notification")]
    public async Task<IActionResult> GetNotificationSettings(CancellationToken cancellationToken)
    {
        var result = await _notificationSettingsService.GetNotificationSettingsAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("notification")]
    public async Task<IActionResult> UpdateNotificationSettings([FromBody] NotificationSettingsDto dto, CancellationToken cancellationToken)
    {
        if (this.UserId == Guid.Empty)
            return HandleResult(EIskele.Application.Common.Results.Result.Failure("UNAUTHORIZED", "Kullanıcı doğrulanamadı."));

        var result = await _notificationSettingsService.UpdateNotificationSettingsAsync(dto, this.UserId, cancellationToken);
        return HandleResult(result);
    }

    public class TestNotificationRequest
    {
        public string ScenarioKey { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public SmtpEmailSettingsDto Settings { get; set; } = new();
    }

    [HttpPost("notification/test")]
    public async Task<IActionResult> TestNotification([FromBody] TestNotificationRequest request, CancellationToken cancellationToken)
    {
        var result = await _smtpEmailSettingsService.SendTestScenarioEmailAsync(request.ScenarioKey, request.Email, request.Settings, cancellationToken);
        return HandleResult(result, "Test bildirimi gönderildi.");
    }

    [HttpGet("security")]
    public async Task<IActionResult> GetSecuritySettings(CancellationToken cancellationToken)
    {
        var result = await _securitySettingsService.GetSecuritySettingsAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("security")]
    public async Task<IActionResult> UpdateSecuritySettings([FromBody] SecuritySettingsDto dto, CancellationToken cancellationToken)
    {
        if (this.UserId == Guid.Empty)
            return HandleResult(EIskele.Application.Common.Results.Result.Failure("UNAUTHORIZED", "Kullanıcı doğrulanamadı."));

        var result = await _securitySettingsService.UpdateSecuritySettingsAsync(dto, this.UserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("payment")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> GetPaymentSettings(CancellationToken cancellationToken)
    {
        var result = await _paymentSettingsService.GetPaymentSettingsAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("payment")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdatePaymentSettings([FromBody] PaymentSettingsDto dto, CancellationToken cancellationToken)
    {
        var result = await _paymentSettingsService.UpdatePaymentSettingsAsync(dto, this.UserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("sms")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> GetSmsSettings(CancellationToken cancellationToken)
    {
        var result = await _smsSettingsService.GetSmsSettingsAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("sms")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdateSmsSettings([FromBody] SmsSettingsDto dto, CancellationToken cancellationToken)
    {
        var result = await _smsSettingsService.UpdateSmsSettingsAsync(dto, this.UserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("sms/test")]
    [Authorize(Roles = "SuperAdmin")]
    public IActionResult TestSmsConnection()
    {
        return HandleResult(EIskele.Application.Common.Results.Result.Success(), "SMS bağlantı testi başarılı (Mock).");
    }

    [HttpPost("sms/send-test")]
    [Authorize(Roles = "SuperAdmin")]
    public IActionResult SendTestSms([FromBody] string phoneNumber)
    {
        return HandleResult(EIskele.Application.Common.Results.Result.Success(), $"Test SMS'i {phoneNumber} numarasına gönderildi (Mock).");
    }

    [HttpGet("maintenance")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> GetMaintenanceModeSettings(CancellationToken cancellationToken)
    {
        var result = await _maintenanceModeSettingsService.GetMaintenanceModeSettingsAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("maintenance")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdateMaintenanceModeSettings([FromBody] MaintenanceModeSettingsDto dto, CancellationToken cancellationToken)
    {
        var result = await _maintenanceModeSettingsService.UpdateMaintenanceModeSettingsAsync(dto, this.UserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("maintenance/enable")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> EnableMaintenanceMode(CancellationToken cancellationToken)
    {
        var result = await _maintenanceModeSettingsService.GetMaintenanceModeSettingsAsync(cancellationToken);
        if (result.IsSuccess && result.Value != null)
        {
            var dto = result.Value;
            dto.MaintenanceModeEnabled = true;
            var updateResult = await _maintenanceModeSettingsService.UpdateMaintenanceModeSettingsAsync(dto, this.UserId, cancellationToken);
            return HandleResult(updateResult, "Bakım modu aktifleştirildi.");
        }
        return HandleResult(EIskele.Application.Common.Results.Result.Failure("UPDATE_FAILED", "Bakım modu güncellenemedi."));
    }

    [HttpPost("maintenance/disable")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> DisableMaintenanceMode(CancellationToken cancellationToken)
    {
        var result = await _maintenanceModeSettingsService.GetMaintenanceModeSettingsAsync(cancellationToken);
        if (result.IsSuccess && result.Value != null)
        {
            var dto = result.Value;
            dto.MaintenanceModeEnabled = false;
            var updateResult = await _maintenanceModeSettingsService.UpdateMaintenanceModeSettingsAsync(dto, this.UserId, cancellationToken);
            return HandleResult(updateResult, "Bakım modu deaktif edildi.");
        }
        return HandleResult(EIskele.Application.Common.Results.Result.Failure("UPDATE_FAILED", "Bakım modu güncellenemedi."));
    }

    [HttpGet("audit-logs")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetSettingsAuditLogs(CancellationToken cancellationToken)
    {
        var result = await _settingsAuditLogService.GetSettingsAuditLogsAsync(cancellationToken);
        return HandleResult(result);
    }
}
