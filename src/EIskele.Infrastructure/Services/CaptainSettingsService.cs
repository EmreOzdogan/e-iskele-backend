using EIskele.Application.Common.Results;
using EIskele.Application.Settings;
using EIskele.Application.Settings.DTOs;
using EIskele.Domain.Entities;
using EIskele.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Services;

public class CaptainSettingsService : ICaptainSettingsService
{
    private readonly EIskeleDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CaptainSettingsService(EIskeleDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Result<CaptainSettingsDto>> GetSettingsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Captain)
                .ThenInclude(c => c!.Company)
            .Include(u => u.NotificationPreferences)
            .Include(u => u.LegalAgreements)
            .Include(u => u.SecurityEvents)
            .Include(u => u.ActiveSessions)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null || user.Captain == null)
            return Result<CaptainSettingsDto>.Failure("NOT_FOUND", "Kaptan bulunamadı.");

        var captain = user.Captain;

        var dto = new CaptainSettingsDto
        {
            Profile = new CaptainProfileDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Phone = user.PhoneNumber ?? string.Empty,
                ShortBio = captain.Bio ?? string.Empty,
                Languages = Array.Empty<string>()
            },
            Application = new CaptainApplicationDto
            {
                Status = captain.Status ?? "Unknown",
                SubmittedAt = user.CreatedAt.ToString("o"),
                DocumentStatus = "Approved", // Simplified
                VerificationLevel = "Pro"
            },
            Payment = new CaptainPaymentDto
            {
                BankName = string.Empty,
                Iban = captain.Iban ?? string.Empty,
                AccountHolderName = string.Empty
            },
            Security = new CaptainSecurityDto
            {
                LastPasswordChange = user.LastPasswordChangedAt?.ToString("o") ?? "N/A",
                TwoFactorEnabled = user.TwoFactorEnabled,
                ActiveSessions = user.ActiveSessions.Select(s => new ActiveSessionDto
                {
                    Id = s.Id.ToString(),
                    Device = s.Device,
                    Location = s.Location,
                    IpAddress = s.IpAddress,
                    LastAccess = s.LastAccess.ToString("o"),
                    IsCurrent = s.IsCurrent
                }).ToList(),
                RecentEvents = user.SecurityEvents.OrderByDescending(e => e.CreatedAt).Take(5).Select(e => new SecurityEventDto
                {
                    Id = e.Id.ToString(),
                    Event = e.Title,
                    Date = e.CreatedAt.ToString("o"),
                    IpAddress = e.IpAddress,
                    Device = e.UserAgent
                }).ToList()
            },
            Notifications = new CaptainNotificationsDto
            {
                Email = user.NotificationPreferences.FirstOrDefault(p => p.Category == "general")?.Email ?? true,
                Sms = user.NotificationPreferences.FirstOrDefault(p => p.Category == "general")?.Sms ?? false,
                Whatsapp = user.NotificationPreferences.FirstOrDefault(p => p.Category == "general")?.Whatsapp ?? false,
                Push = user.NotificationPreferences.FirstOrDefault(p => p.Category == "general")?.InApp ?? true,
                Promotional = false
            },
            Legal = new CaptainLegalDto
            {
                ContractStatus = "accepted",
                ContractDate = user.LegalAgreements.FirstOrDefault(a => a.AgreementName == "KVKK")?.CreatedAt.ToString("o") ?? "2024-01-01T00:00:00Z",
                Permissions = new LegalPermissionsDto
                {
                    Kvkk = user.LegalAgreements.Any(a => a.AgreementName == "KVKK" && a.Status == "accepted"),
                    Commercial = user.LegalAgreements.Any(a => a.AgreementName == "Commercial" && a.Status == "accepted")
                }
            }
        };

        return Result<CaptainSettingsDto>.Success(dto);
    }

    public async Task<Result<bool>> UpdateProfileAsync(Guid userId, UpdateCaptainProfileDto request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Captain)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null || user.Captain == null)
            return Result<bool>.Failure("NOT_FOUND", "Kaptan bulunamadı.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.Phone;
        
        user.Captain.Bio = request.ShortBio;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UpdatePaymentAsync(Guid userId, UpdateCaptainPaymentDto request, CancellationToken cancellationToken)
    {
        var captain = await _context.Captains.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        if (captain == null)
            return Result<bool>.Failure("NOT_FOUND", "Kaptan bulunamadı.");

        captain.Iban = request.Iban;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangeCaptainPasswordDto request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result<bool>.Failure("NOT_FOUND", "Kullanıcı bulunamadı.");

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        
        if (result.Succeeded)
        {
            user.LastPasswordChangedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return Result<bool>.Success(true);
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return Result<bool>.Failure("VALIDATION_ERROR", $"Şifre değiştirilemedi: {errors}");
    }

    public async Task<Result<bool>> RevokeOtherSessionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var sessions = await _context.UserActiveSessions
            .Where(s => s.UserId == userId && !s.IsCurrent)
            .ToListAsync(cancellationToken);

        _context.UserActiveSessions.RemoveRange(sessions);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> SaveNotificationPreferencesAsync(Guid userId, UpdateNotificationPreferencesDto request, CancellationToken cancellationToken)
    {
        var pref = await _context.UserNotificationPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId && p.Category == "general", cancellationToken);

        if (pref == null)
        {
            pref = new UserNotificationPreference
            {
                UserId = userId,
                Category = "general"
            };
            _context.UserNotificationPreferences.Add(pref);
        }

        pref.Email = request.Email;
        pref.Sms = request.Sms;
        pref.Whatsapp = request.Whatsapp;
        pref.InApp = request.Push;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
