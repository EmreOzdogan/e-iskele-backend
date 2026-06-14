namespace EIskele.Application.Payments;

public class AdminPaymentSummaryMetricsDto
{
    public int TotalTransactions { get; set; }
    public decimal TodayCollections { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal PlatformCommission { get; set; }
    public decimal ServiceFee { get; set; }
    public decimal CaptainPayouts { get; set; }
    public int PendingPayments { get; set; }
    public int PendingRefunds { get; set; }
    
    // Additional tab counts
    public int PaidPayments { get; set; }
    public int FailedPayments { get; set; }
    public int RefundedPayments { get; set; }
    public int PartiallyRefundedPayments { get; set; }
    public int PayoutPendingPayments { get; set; }
}
