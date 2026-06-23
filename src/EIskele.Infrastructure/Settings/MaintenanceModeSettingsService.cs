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

public class MaintenanceModeSettingsService : IMaintenanceModeSettingsService
{
    private readonly EIskeleDbContext _dbContext;

    public MaintenanceModeSettingsService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<MaintenanceModeSettingsDto>> GetMaintenanceModeSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
        
        bool GetValueBool(string key, bool def) => bool.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;
        string GetValueString(string key, string def) => settings.FirstOrDefault(s => s.Key == key)?.Value ?? def;

        var dto = new MaintenanceModeSettingsDto
        {
            MaintenanceModeEnabled = GetValueBool("Maintenance.MaintenanceModeEnabled", false),
            MaintenanceMessage = GetValueString("Maintenance.MaintenanceMessage", "e-iskele kÄ±sa sÃ¼reli bakÄ±m nedeniyle geÃ§ici olarak hizmet veremiyor."),
            MaintenanceAffectsCustomerWeb = GetValueBool("Maintenance.MaintenanceAffectsCustomerWeb", true),
            MaintenanceAffectsCaptainHub = GetValueBool("Maintenance.MaintenanceAffectsCaptainHub", true),
            MaintenanceAffectsAdminPanel = GetValueBool("Maintenance.MaintenanceAffectsAdminPanel", false),
            MaintenanceAffectsPublicApi = GetValueBool("Maintenance.MaintenanceAffectsPublicApi", true)
        };

        return Result<MaintenanceModeSettingsDto>.Success(dto);
    }

    public async Task<Result> UpdateMaintenanceModeSettingsAsync(MaintenanceModeSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.ToListAsync(cancellationToken);

        void UpdateValue(string key, string value, string valueType)
        {
            var setting = settings.FirstOrDefault(s => s.Key == key);
            if (setting == null)
            {
                _dbContext.SystemSettings.Add(new Domain.Entities.SystemSetting 
                { 
                    Key = key, Value = value, ValueType = valueType == "Number" ? SystemSettingValueType.Int : Enum.Parse<SystemSettingValueType>(valueType, true), Group = "Maintenance", Description = "", IsEditable = true, CreatedAt = DateTime.UtcNow, CreatedBy = currentUserId 
                });
            }
            else if (setting.Value != value)
            {
                setting.Value = value; setting.UpdatedAt = DateTime.UtcNow; setting.UpdatedBy = currentUserId;
            }
        }

        UpdateValue("Maintenance.MaintenanceModeEnabled", dto.MaintenanceModeEnabled.ToString(), "Boolean");
        UpdateValue("Maintenance.MaintenanceMessage", dto.MaintenanceMessage, "String");
        UpdateValue("Maintenance.MaintenanceAffectsCustomerWeb", dto.MaintenanceAffectsCustomerWeb.ToString(), "Boolean");
        UpdateValue("Maintenance.MaintenanceAffectsCaptainHub", dto.MaintenanceAffectsCaptainHub.ToString(), "Boolean");
        UpdateValue("Maintenance.MaintenanceAffectsAdminPanel", dto.MaintenanceAffectsAdminPanel.ToString(), "Boolean");
        UpdateValue("Maintenance.MaintenanceAffectsPublicApi", dto.MaintenanceAffectsPublicApi.ToString(), "Boolean");

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
