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

public class PaymentSettingsService : IPaymentSettingsService
{
    private readonly EIskeleDbContext _dbContext;

    public PaymentSettingsService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PaymentSettingsDto>> GetPaymentSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
        
        bool GetValueBool(string key, bool def) => bool.TryParse(settings.FirstOrDefault(s => s.Key == key)?.Value, out var val) ? val : def;
        string GetValueString(string key, string def) => settings.FirstOrDefault(s => s.Key == key)?.Value ?? def;

        var dto = new PaymentSettingsDto
        {
            PaymentEnabled = GetValueBool("Payment.PaymentEnabled", false),
            PaymentProvider = GetValueString("Payment.PaymentProvider", "none"),
            PaymentTestMode = GetValueBool("Payment.PaymentTestMode", true),
            Require3DSecure = GetValueBool("Payment.Require3DSecure", true),
            DepositPaymentEnabled = GetValueBool("Payment.DepositPaymentEnabled", true),
            FullPaymentEnabled = GetValueBool("Payment.FullPaymentEnabled", false),
            RefundManagementEnabled = GetValueBool("Payment.RefundManagementEnabled", false)
        };

        return Result<PaymentSettingsDto>.Success(dto);
    }

    public async Task<Result> UpdatePaymentSettingsAsync(PaymentSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default)
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
                    Group = "Payment", Description = "", IsEditable = true, CreatedAt = DateTime.UtcNow, CreatedBy = currentUserId 
                });
            }
            else if (setting.Value != value)
            {
                setting.Value = value; setting.UpdatedAt = DateTime.UtcNow; setting.UpdatedBy = currentUserId;
            }
        }

        UpdateValue("Payment.PaymentEnabled", dto.PaymentEnabled.ToString(), "Boolean");
        UpdateValue("Payment.PaymentProvider", dto.PaymentProvider, "String");
        UpdateValue("Payment.PaymentTestMode", dto.PaymentTestMode.ToString(), "Boolean");
        UpdateValue("Payment.Require3DSecure", dto.Require3DSecure.ToString(), "Boolean");
        UpdateValue("Payment.DepositPaymentEnabled", dto.DepositPaymentEnabled.ToString(), "Boolean");
        UpdateValue("Payment.FullPaymentEnabled", dto.FullPaymentEnabled.ToString(), "Boolean");
        UpdateValue("Payment.RefundManagementEnabled", dto.RefundManagementEnabled.ToString(), "Boolean");

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
