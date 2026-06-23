using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Common.Settings;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Settings;

public class NotificationSettingsService : INotificationSettingsService
{
    private readonly EIskeleDbContext _dbContext;

    public NotificationSettingsService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<NotificationSettingsDto>> GetNotificationSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
        
        bool GetValueBool(string key, bool def) => bool.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;

        var dto = new NotificationSettingsDto
        {
            EmailNotificationsEnabled = GetValueBool("Notification.EmailEnabled", true),
            SmsNotificationsEnabled = GetValueBool("Notification.SmsEnabled", false),
            PushNotificationsEnabled = GetValueBool("Notification.PushEnabled", false),
            WhatsAppNotificationsEnabled = GetValueBool("Notification.WhatsAppEnabled", false),
            InAppNotificationsEnabled = GetValueBool("Notification.InAppEnabled", false),
            Scenarios = new List<NotificationScenarioDto>
            {
                new() { Key = "UserRegistered", Name = "KullanÄ±cÄ± kayÄ±t oldu", Channels = new() { "email" }, Status = "ready" },
                new() { Key = "CaptainApplicationReceived", Name = "Kaptan baÅŸvurusu alÄ±ndÄ±", Channels = new() { "email" }, Status = "ready" },
                new() { Key = "CaptainApplicationApproved", Name = "Kaptan baÅŸvurusu onaylandÄ±", Channels = new() { "email", "sms" }, Status = "ready" },
                new() { Key = "MissingDocumentRequested", Name = "Eksik belge istendi", Channels = new() { "email" }, Status = "ready" },
                new() { Key = "BoatApproved", Name = "Tekne onaylandÄ±", Channels = new() { "email" }, Status = "ready" },
                new() { Key = "ReservationCreated", Name = "Rezervasyon oluÅŸturuldu", Channels = new() { "email", "sms" }, Status = "ready" },
                new() { Key = "ReservationApproved", Name = "Rezervasyon onaylandÄ±", Channels = new() { "email", "sms" }, Status = "ready" },
                new() { Key = "ReservationRejected", Name = "Rezervasyon reddedildi", Channels = new() { "email" }, Status = "ready" },
                new() { Key = "PaymentSuccessful", Name = "Ã–deme baÅŸarÄ±lÄ±", Channels = new() { "email" }, Status = "preparation" },
                new() { Key = "TourReminder24h", Name = "Turdan 24 saat Ã¶nce hatÄ±rlatma", Channels = new() { "email", "push" }, Status = "preparation" }
            }
        };

        return Result<NotificationSettingsDto>.Success(dto);
    }

    public async Task<Result> UpdateNotificationSettingsAsync(NotificationSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.ToListAsync(cancellationToken);

        void UpdateValue(string key, string value, string valueType)
        {
            var setting = settings.FirstOrDefault(s => s.Key == key);
            if (setting == null)
            {
                _dbContext.SystemSettings.Add(new Domain.Entities.SystemSetting
                {
                    Key = key,
                    Value = value,
                    ValueType = Enum.Parse<SystemSettingValueType>(valueType, true),
                    Group = "Notification",
                    Description = "",
                    IsEditable = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId
                });
            }
            else if (setting.Value != value)
            {
                setting.Value = value;
                setting.UpdatedAt = DateTime.UtcNow;
                setting.UpdatedBy = currentUserId;
            }
        }

        UpdateValue("Notification.EmailEnabled", dto.EmailNotificationsEnabled.ToString(), "Boolean");
        UpdateValue("Notification.SmsEnabled", dto.SmsNotificationsEnabled.ToString(), "Boolean");
        UpdateValue("Notification.PushEnabled", dto.PushNotificationsEnabled.ToString(), "Boolean");
        UpdateValue("Notification.WhatsAppEnabled", dto.WhatsAppNotificationsEnabled.ToString(), "Boolean");
        UpdateValue("Notification.InAppEnabled", dto.InAppNotificationsEnabled.ToString(), "Boolean");

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
