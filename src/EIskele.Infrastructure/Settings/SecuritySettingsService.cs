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

public class SecuritySettingsService : ISecuritySettingsService
{
    private readonly EIskeleDbContext _dbContext;

    public SecuritySettingsService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<SecuritySettingsDto>> GetSecuritySettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
        
        int GetValueInt(string key, int def) => int.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;
        bool GetValueBool(string key, bool def) => bool.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;

        var dto = new SecuritySettingsDto
        {
            AdminSessionMinutes = GetValueInt("Security.AdminSessionMinutes", 60),
            RefreshTokenDays = GetValueInt("Security.RefreshTokenDays", 7),
            MaxFailedLoginAttempts = GetValueInt("Security.MaxFailedLoginAttempts", 5),
            AccountLockoutMinutes = GetValueInt("Security.AccountLockoutMinutes", 15),
            PasswordMinimumLength = GetValueInt("Security.PasswordMinimumLength", 8),
            RequirePasswordComplexity = GetValueBool("Security.RequirePasswordComplexity", true),
            AuditLogEnabled = GetValueBool("Security.AuditLogEnabled", true)
        };

        return Result<SecuritySettingsDto>.Success(dto);
    }

    public async Task<Result> UpdateSecuritySettingsAsync(SecuritySettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
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
                    ValueType = valueType == "Number" ? SystemSettingValueType.Int : Enum.Parse<SystemSettingValueType>(valueType, true),
                    Group = "Security",
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

        UpdateValue("Security.AdminSessionMinutes", dto.AdminSessionMinutes.ToString(), "Number");
        UpdateValue("Security.RefreshTokenDays", dto.RefreshTokenDays.ToString(), "Number");
        UpdateValue("Security.MaxFailedLoginAttempts", dto.MaxFailedLoginAttempts.ToString(), "Number");
        UpdateValue("Security.AccountLockoutMinutes", dto.AccountLockoutMinutes.ToString(), "Number");
        UpdateValue("Security.PasswordMinimumLength", dto.PasswordMinimumLength.ToString(), "Number");
        UpdateValue("Security.RequirePasswordComplexity", dto.RequirePasswordComplexity.ToString(), "Boolean");
        UpdateValue("Security.AuditLogEnabled", dto.AuditLogEnabled.ToString(), "Boolean");

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
