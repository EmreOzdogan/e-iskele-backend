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
}
