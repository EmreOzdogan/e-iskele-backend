using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Common.Settings;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Settings;

public class SmsSettingsService : ISmsSettingsService
{
    private readonly EIskeleDbContext _dbContext;

    public SmsSettingsService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<SmsSettingsDto>> GetSmsSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
        
        bool GetValueBool(string key, bool def) => bool.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;
        string GetValueString(string key, string def) => settings.FirstOrDefault(s => s.Key == key)?.Value ?? def;

        var dto = new SmsSettingsDto
        {
            SmsEnabled = GetValueBool("Sms.SmsEnabled", false),
            SmsProvider = GetValueString("Sms.SmsProvider", "none"),
            SmsSenderTitle = GetValueString("Sms.SmsSenderTitle", "EISKELE")
        };

        return Result<SmsSettingsDto>.Success(dto);
    }

    public async Task<Result> UpdateSmsSettingsAsync(SmsSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.ToListAsync(cancellationToken);

        void UpdateValue(string key, string value, string valueType)
        {
            var setting = settings.FirstOrDefault(s => s.Key == key);
            if (setting == null)
            {
                _dbContext.SystemSettings.Add(new Domain.Entities.SystemSetting 
                { 
                    Key = key, Value = value, ValueType = valueType == "Number" ? SystemSettingValueType.Int : Enum.Parse<SystemSettingValueType>(valueType, true), Group = "Sms", Description = "", IsEditable = true, CreatedAt = DateTime.UtcNow, CreatedBy = currentUserId 
                });
            }
            else if (setting.Value != value)
            {
                setting.Value = value; setting.UpdatedAt = DateTime.UtcNow; setting.UpdatedBy = currentUserId;
            }
        }

        UpdateValue("Sms.SmsEnabled", dto.SmsEnabled.ToString(), "Boolean");
        UpdateValue("Sms.SmsProvider", dto.SmsProvider, "String");
        UpdateValue("Sms.SmsSenderTitle", dto.SmsSenderTitle, "String");

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
