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
    private readonly ISettingsService _settingsService;

    public SystemSettingsController(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet("general")]
    public async Task<IActionResult> GetGeneralSettings(CancellationToken cancellationToken)
    {
        var result = await _settingsService.GetGeneralSettingsAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("general")]
    public async Task<IActionResult> UpdateGeneralSettings([FromBody] SystemSettingsDto dto, CancellationToken cancellationToken)
    {
        var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentUserId = currentUserIdStr != null ? Guid.Parse(currentUserIdStr) : Guid.Empty;

        var result = await _settingsService.UpdateGeneralSettingsAsync(dto, currentUserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("reservation-rules")]
    public async Task<IActionResult> GetReservationRules(CancellationToken cancellationToken)
    {
        var result = await _settingsService.GetReservationRulesSettingsAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("reservation-rules")]
    public async Task<IActionResult> UpdateReservationRules([FromBody] ReservationRulesSettingsDto dto, CancellationToken cancellationToken)
    {
        var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentUserId = currentUserIdStr != null ? Guid.Parse(currentUserIdStr) : Guid.Empty;

        var result = await _settingsService.UpdateReservationRulesSettingsAsync(dto, currentUserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("commission-finance")]
    public async Task<IActionResult> GetCommissionFinance(CancellationToken cancellationToken)
    {
        var result = await _settingsService.GetCommissionFinanceSettingsAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("commission-finance")]
    public async Task<IActionResult> UpdateCommissionFinance([FromBody] CommissionFinanceSettingsDto dto, CancellationToken cancellationToken)
    {
        var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentUserId = currentUserIdStr != null ? Guid.Parse(currentUserIdStr) : Guid.Empty;

        var result = await _settingsService.UpdateCommissionFinanceSettingsAsync(dto, currentUserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("smtp-email")]
    public async Task<IActionResult> GetSmtpEmail(CancellationToken cancellationToken)
    {
        var result = await _settingsService.GetSmtpEmailSettingsAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("smtp-email")]
    public async Task<IActionResult> UpdateSmtpEmail([FromBody] SmtpEmailSettingsDto dto, CancellationToken cancellationToken)
    {
        var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentUserId = currentUserIdStr != null ? Guid.Parse(currentUserIdStr) : Guid.Empty;

        var result = await _settingsService.UpdateSmtpEmailSettingsAsync(dto, currentUserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("smtp-email/test")]
    public async Task<IActionResult> TestSmtpEmail([FromBody] SmtpEmailSettingsDto dto, CancellationToken cancellationToken)
    {
        var result = await _settingsService.TestSmtpConnectionAsync(dto, cancellationToken);
        if (!result.IsSuccess)
        {
            return Ok(new { success = false, message = result.Error.Message });
        }
        return Ok(new { success = true, message = "SMTP bağlantısı başarılı." });
    }

    public class SendTestEmailRequest
    {
        public string Email { get; set; } = string.Empty;
        public SmtpEmailSettingsDto Settings { get; set; } = new();
    }

    [HttpPost("smtp-email/send-test")]
    public async Task<IActionResult> SendTestEmail([FromBody] SendTestEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await _settingsService.SendTestEmailAsync(request.Email, request.Settings, cancellationToken);
        if (result.IsSuccess)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpGet("notification")]
    public async Task<IActionResult> GetNotificationSettings(CancellationToken cancellationToken)
    {
        var result = await _settingsService.GetNotificationSettingsAsync(cancellationToken);
        if (result.IsSuccess)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPut("notification")]
    public async Task<IActionResult> UpdateNotificationSettings([FromBody] NotificationSettingsDto dto, CancellationToken cancellationToken)
    {
        var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentUserId = currentUserIdStr != null ? Guid.Parse(currentUserIdStr) : Guid.Empty;

        if (currentUserId == Guid.Empty)
            return Unauthorized();

        var result = await _settingsService.UpdateNotificationSettingsAsync(dto, currentUserId, cancellationToken);
        if (result.IsSuccess)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("notification/test")]
    public IActionResult TestNotification([FromBody] object payload)
    {
        // Mock successful test notification for now
        return Ok(EIskele.Application.Common.Results.Result.Success());
    }

    [HttpGet("security")]
    public async Task<IActionResult> GetSecuritySettings(CancellationToken cancellationToken)
    {
        var result = await _settingsService.GetSecuritySettingsAsync(cancellationToken);
        if (result.IsSuccess)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPut("security")]
    public async Task<IActionResult> UpdateSecuritySettings([FromBody] SecuritySettingsDto dto, CancellationToken cancellationToken)
    {
        var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentUserId = currentUserIdStr != null ? Guid.Parse(currentUserIdStr) : Guid.Empty;

        if (currentUserId == Guid.Empty)
            return Unauthorized();

        var result = await _settingsService.UpdateSecuritySettingsAsync(dto, currentUserId, cancellationToken);
        if (result.IsSuccess)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpGet("payment")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> GetPaymentSettings(CancellationToken cancellationToken)
    {
        var result = await _settingsService.GetPaymentSettingsAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("payment")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdatePaymentSettings([FromBody] PaymentSettingsDto dto, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var result = await _settingsService.UpdatePaymentSettingsAsync(dto, currentUserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("sms")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> GetSmsSettings(CancellationToken cancellationToken)
    {
        var result = await _settingsService.GetSmsSettingsAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("sms")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdateSmsSettings([FromBody] SmsSettingsDto dto, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var result = await _settingsService.UpdateSmsSettingsAsync(dto, currentUserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("sms/test")]
    [Authorize(Roles = "SuperAdmin")]
    public IActionResult TestSmsConnection()
    {
        // MVP için SMS entegrasyonu tamamlanana kadar mock başarılı dönülüyor.
        return Ok(new { success = true, message = "SMS bağlantı testi başarılı (Mock)." });
    }

    [HttpPost("sms/send-test")]
    [Authorize(Roles = "SuperAdmin")]
    public IActionResult SendTestSms([FromBody] string phoneNumber)
    {
        // MVP için SMS entegrasyonu tamamlanana kadar mock başarılı dönülüyor.
        return Ok(new { success = true, message = $"Test SMS'i {phoneNumber} numarasına gönderildi (Mock)." });
    }

    [HttpGet("maintenance")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> GetMaintenanceModeSettings(CancellationToken cancellationToken)
    {
        var result = await _settingsService.GetMaintenanceModeSettingsAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("maintenance")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdateMaintenanceModeSettings([FromBody] MaintenanceModeSettingsDto dto, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var result = await _settingsService.UpdateMaintenanceModeSettingsAsync(dto, currentUserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("maintenance/enable")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> EnableMaintenanceMode(CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var result = await _settingsService.GetMaintenanceModeSettingsAsync(cancellationToken);
        if (result.IsSuccess && result.Value != null)
        {
            var dto = result.Value;
            dto.MaintenanceModeEnabled = true;
            await _settingsService.UpdateMaintenanceModeSettingsAsync(dto, currentUserId, cancellationToken);
            return Ok(new { success = true, message = "Bakım modu aktifleştirildi." });
        }
        return BadRequest(new { success = false, message = "Bakım modu güncellenemedi." });
    }

    [HttpPost("maintenance/disable")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> DisableMaintenanceMode(CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var result = await _settingsService.GetMaintenanceModeSettingsAsync(cancellationToken);
        if (result.IsSuccess && result.Value != null)
        {
            var dto = result.Value;
            dto.MaintenanceModeEnabled = false;
            await _settingsService.UpdateMaintenanceModeSettingsAsync(dto, currentUserId, cancellationToken);
            return Ok(new { success = true, message = "Bakım modu deaktif edildi." });
        }
        return BadRequest(new { success = false, message = "Bakım modu güncellenemedi." });
    }

    [HttpGet("audit-logs")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetSettingsAuditLogs(CancellationToken cancellationToken)
    {
        var result = await _settingsService.GetSettingsAuditLogsAsync(cancellationToken);
        return HandleResult(result);
    }
}
