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

public class GeneralSettingsService : IGeneralSettingsService
{
    private readonly EIskeleDbContext _dbContext;

    public GeneralSettingsService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<SystemSettingsDto>> GetGeneralSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
        string GetValue(string key, string def) => settings.FirstOrDefault(s => s.Key == key)?.Value ?? def;

        var dto = new SystemSettingsDto
        {
            PlatformName = GetValue("General.PlatformName", "e-iskele"),
            Slogan = GetValue("General.Slogan", ""),
            DefaultLanguage = GetValue("General.DefaultLanguage", "tr"),
            DefaultCurrency = GetValue("General.DefaultCurrency", "TRY"),
            Timezone = GetValue("General.Timezone", "Europe/Istanbul"),
            CustomerWebUrl = GetValue("General.CustomerWebUrl", ""),
            CaptainHubUrl = GetValue("General.CaptainHubUrl", ""),
            AdminPanelUrl = GetValue("General.AdminPanelUrl", ""),
            ApiBaseUrl = GetValue("General.ApiBaseUrl", ""),
            CdnUrl = GetValue("General.CdnUrl", ""),
            SupportEmail = GetValue("General.SupportEmail", ""),
            SupportPhone = GetValue("General.SupportPhone", "")
        };

        return Result<SystemSettingsDto>.Success(dto);
    }

    public async Task<Result> UpdateGeneralSettingsAsync(SystemSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.ToListAsync(cancellationToken);

        void UpdateValue(string key, string? value)
        {
            var setting = settings.FirstOrDefault(s => s.Key == key);
            if (setting == null)
            {
                if (value != null)
                {
                    _dbContext.SystemSettings.Add(new Domain.Entities.SystemSetting
                    {
                        Key = key,
                        Value = value,
                        ValueType = SystemSettingValueType.String,
                        Group = "General",
                        Description = "",
                        IsEditable = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = currentUserId
                    });
                }
            }
            else
            {
                if (setting.Value != value)
                {
                    setting.Value = value ?? "";
                    setting.UpdatedAt = DateTime.UtcNow;
                    setting.UpdatedBy = currentUserId;
                }
            }
        }

        UpdateValue("General.PlatformName", dto.PlatformName);
        UpdateValue("General.Slogan", dto.Slogan);
        UpdateValue("General.DefaultLanguage", dto.DefaultLanguage);
        UpdateValue("General.DefaultCurrency", dto.DefaultCurrency);
        UpdateValue("General.Timezone", dto.Timezone);
        UpdateValue("General.CustomerWebUrl", dto.CustomerWebUrl);
        UpdateValue("General.CaptainHubUrl", dto.CaptainHubUrl);
        UpdateValue("General.AdminPanelUrl", dto.AdminPanelUrl);
        UpdateValue("General.ApiBaseUrl", dto.ApiBaseUrl);
        UpdateValue("General.CdnUrl", dto.CdnUrl);
        UpdateValue("General.SupportEmail", dto.SupportEmail);
        UpdateValue("General.SupportPhone", dto.SupportPhone);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
