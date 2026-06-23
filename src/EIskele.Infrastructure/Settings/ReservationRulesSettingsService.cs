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

public class ReservationRulesSettingsService : IReservationRulesSettingsService
{
    private readonly EIskeleDbContext _dbContext;

    public ReservationRulesSettingsService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<ReservationRulesSettingsDto>> GetReservationRulesSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
        
        string GetValueStr(string key, string def) => settings.FirstOrDefault(s => s.Key == key)?.Value ?? def;
        int GetValueInt(string key, int def) => int.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;
        bool GetValueBool(string key, bool def) => bool.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;

        var dto = new ReservationRulesSettingsDto
        {
            DefaultReservationApprovalType = GetValueStr("Reservation.DefaultReservationApprovalType", "captainApproval"),
            CaptainApprovalTimeoutHours = GetValueInt("Reservation.CaptainApprovalTimeoutHours", 12),
            MinimumReservationLeadTimeHours = GetValueInt("Reservation.MinimumReservationLeadTimeHours", 24),
            MaxAdvanceReservationDays = GetValueInt("Reservation.MaxAdvanceReservationDays", 90),
            CancellationAllowedHoursBeforeTour = GetValueInt("Reservation.CancellationAllowedHoursBeforeTour", 48),
            WeatherPostponeEnabled = GetValueBool("Reservation.WeatherPostponeEnabled", true),
            GuestCountValidationEnabled = GetValueBool("Reservation.GuestCountValidationEnabled", true),
            PreventOverlappingReservations = GetValueBool("Reservation.PreventOverlappingReservations", true)
        };

        return Result<ReservationRulesSettingsDto>.Success(dto);
    }

    public async Task<Result> UpdateReservationRulesSettingsAsync(ReservationRulesSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
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
                    Group = "ReservationRules",
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

        UpdateValue("Reservation.DefaultReservationApprovalType", dto.DefaultReservationApprovalType, "String");
        UpdateValue("Reservation.CaptainApprovalTimeoutHours", dto.CaptainApprovalTimeoutHours.ToString(), "Int");
        UpdateValue("Reservation.MinimumReservationLeadTimeHours", dto.MinimumReservationLeadTimeHours.ToString(), "Int");
        UpdateValue("Reservation.MaxAdvanceReservationDays", dto.MaxAdvanceReservationDays.ToString(), "Int");
        UpdateValue("Reservation.CancellationAllowedHoursBeforeTour", dto.CancellationAllowedHoursBeforeTour.ToString(), "Int");
        UpdateValue("Reservation.WeatherPostponeEnabled", dto.WeatherPostponeEnabled.ToString(), "Boolean");
        UpdateValue("Reservation.GuestCountValidationEnabled", dto.GuestCountValidationEnabled.ToString(), "Boolean");
        UpdateValue("Reservation.PreventOverlappingReservations", dto.PreventOverlappingReservations.ToString(), "Boolean");

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
