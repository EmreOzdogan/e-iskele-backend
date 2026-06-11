using System;

namespace EIskele.Application.Common.Settings;

public class CommissionFinanceSettingsDto
{
    public decimal PlatformCommissionRate { get; set; } = 10;
    public string ServiceFeeType { get; set; } = "percentage";
    public decimal? ServiceFeeRate { get; set; } = 5;
    public decimal? ServiceFeeFixedAmount { get; set; } = 0;
    public decimal MinimumDepositRate { get; set; } = 30;
    public decimal MaximumDepositRate { get; set; } = 100;
    public int PayoutHoldDays { get; set; } = 3;
    public int RefundReviewDays { get; set; } = 7;
    public string CaptainPayoutPeriod { get; set; } = "weekly";
}
