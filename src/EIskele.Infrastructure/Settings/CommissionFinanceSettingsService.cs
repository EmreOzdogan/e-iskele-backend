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

public class CommissionFinanceSettingsService : ICommissionFinanceSettingsService
{
    private readonly EIskeleDbContext _dbContext;

    public CommissionFinanceSettingsService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<CommissionFinanceSettingsDto>> GetCommissionFinanceSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
        
        string GetValueStr(string key, string def) => settings.FirstOrDefault(s => s.Key == key)?.Value ?? def;
        int GetValueInt(string key, int def) => int.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;
        decimal GetValueDecimal(string key, decimal def) => decimal.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;

        var dto = new CommissionFinanceSettingsDto
        {
            PlatformCommissionRate = GetValueDecimal("Finance.PlatformCommissionRate", 10m),
            ServiceFeeType = GetValueStr("Finance.ServiceFeeType", "percentage"),
            ServiceFeeRate = GetValueDecimal("Finance.ServiceFeeRate", 5m),
            ServiceFeeFixedAmount = GetValueDecimal("Finance.ServiceFeeFixedAmount", 0m),
            MinimumDepositRate = GetValueDecimal("Finance.MinimumDepositRate", 30m),
            MaximumDepositRate = GetValueDecimal("Finance.MaximumDepositRate", 100m),
            PayoutHoldDays = GetValueInt("Finance.PayoutHoldDays", 3),
            RefundReviewDays = GetValueInt("Finance.RefundReviewDays", 7),
            CaptainPayoutPeriod = GetValueStr("Finance.CaptainPayoutPeriod", "weekly")
        };

        return Result<CommissionFinanceSettingsDto>.Success(dto);
    }

    public async Task<Result> UpdateCommissionFinanceSettingsAsync(CommissionFinanceSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
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
                    Group = "CommissionFinance",
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

        UpdateValue("Finance.PlatformCommissionRate", dto.PlatformCommissionRate.ToString(), "Decimal");
        UpdateValue("Finance.ServiceFeeType", dto.ServiceFeeType, "String");
        UpdateValue("Finance.ServiceFeeRate", (dto.ServiceFeeRate ?? 0).ToString(), "Decimal");
        UpdateValue("Finance.ServiceFeeFixedAmount", (dto.ServiceFeeFixedAmount ?? 0).ToString(), "Decimal");
        UpdateValue("Finance.MinimumDepositRate", dto.MinimumDepositRate.ToString(), "Decimal");
        UpdateValue("Finance.MaximumDepositRate", dto.MaximumDepositRate.ToString(), "Decimal");
        UpdateValue("Finance.PayoutHoldDays", dto.PayoutHoldDays.ToString(), "Int");
        UpdateValue("Finance.RefundReviewDays", dto.RefundReviewDays.ToString(), "Int");
        UpdateValue("Finance.CaptainPayoutPeriod", dto.CaptainPayoutPeriod, "String");

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
