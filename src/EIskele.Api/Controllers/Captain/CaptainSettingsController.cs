using EIskele.Api.Controllers;
using EIskele.Application.Settings;
using EIskele.Application.Settings.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers.Captain;

[Authorize(Roles = "Captain")]
[Route("api/captain/settings")]
public class CaptainSettingsController : BaseController
{
    private readonly ICaptainSettingsService _settingsService;

    public CaptainSettingsController(ICaptainSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSettings(CancellationToken cancellationToken)
    {
        var result = await _settingsService.GetSettingsAsync(UserId, cancellationToken);
        return HandleResult(result);
    }

    [AllowAnonymous]
    [HttpGet("test-json")]
    public IActionResult TestJson()
    {
        var dto = new CaptainSettingsDto();
        return Ok(EIskele.Shared.Responses.ApiResponse<CaptainSettingsDto>.CreateSuccess(dto));
    }


    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateCaptainProfileDto request, CancellationToken cancellationToken)
    {
        var result = await _settingsService.UpdateProfileAsync(UserId, request, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("payment")]
    public async Task<IActionResult> UpdatePayment([FromBody] UpdateCaptainPaymentDto request, CancellationToken cancellationToken)
    {
        var result = await _settingsService.UpdatePaymentAsync(UserId, request, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangeCaptainPasswordDto request, CancellationToken cancellationToken)
    {
        var result = await _settingsService.ChangePasswordAsync(UserId, request, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("revoke-other-sessions")]
    public async Task<IActionResult> RevokeOtherSessions(CancellationToken cancellationToken)
    {
        var result = await _settingsService.RevokeOtherSessionsAsync(UserId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("notifications")]
    public async Task<IActionResult> UpdateNotificationPreferences([FromBody] UpdateNotificationPreferencesDto request, CancellationToken cancellationToken)
    {
        var result = await _settingsService.SaveNotificationPreferencesAsync(UserId, request, cancellationToken);
        return HandleResult(result);
    }
}
