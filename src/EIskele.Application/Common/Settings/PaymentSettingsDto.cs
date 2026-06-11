using System;

namespace EIskele.Application.Common.Settings;

public class PaymentSettingsDto
{
    public bool PaymentEnabled { get; set; } = false;
    public string PaymentProvider { get; set; } = "none";
    public bool PaymentTestMode { get; set; } = true;
    public bool Require3DSecure { get; set; } = true;
    public bool DepositPaymentEnabled { get; set; } = true;
    public bool FullPaymentEnabled { get; set; } = false;
    public bool RefundManagementEnabled { get; set; } = false;
}
