using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EIskele.Application.Common.Results;
using EIskele.Application.Payments;
using EIskele.Domain.Enums;

namespace EIskele.Infrastructure.Services;

public partial class PaymentService
{
    public async Task<Result<AdminPaymentSummaryMetricsDto>> GetAdminPaymentsSummaryAsync(CancellationToken cancellationToken = default)
    {
        var paymentsQuery = _context.Payments.AsNoTracking();

        var totalTransactions = await paymentsQuery.CountAsync(cancellationToken);
        var todayCollections = await paymentsQuery
            .Where(p => p.Status == PaymentStatus.Paid && p.PaidAt >= DateTime.UtcNow.Date)
            .SumAsync(p => p.Amount, cancellationToken);
            
        var monthlyRevenue = await paymentsQuery
            .Where(p => p.Status == PaymentStatus.Paid && p.PaidAt >= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1))
            .SumAsync(p => p.Amount, cancellationToken);

        var platformCommission = await paymentsQuery
            .Where(p => p.Status == PaymentStatus.Paid)
            .SumAsync(p => p.PlatformCommission, cancellationToken);

        var serviceFee = await paymentsQuery
            .Where(p => p.Status == PaymentStatus.Paid)
            .SumAsync(p => p.ServiceFeeAmount, cancellationToken);

        var captainPayouts = await paymentsQuery
            .Where(p => p.PayoutStatus == PayoutStatus.Paid)
            .SumAsync(p => p.CaptainEarnings, cancellationToken);

        var pendingPayments = await paymentsQuery
            .CountAsync(p => p.Status == PaymentStatus.Pending, cancellationToken);

        var pendingRefunds = await paymentsQuery
            .CountAsync(p => p.RefundStatus == RefundStatus.Pending, cancellationToken);

        var result = new AdminPaymentSummaryMetricsDto
        {
            TotalTransactions = totalTransactions,
            TodayCollections = todayCollections,
            MonthlyRevenue = monthlyRevenue,
            PlatformCommission = platformCommission,
            ServiceFee = serviceFee,
            CaptainPayouts = captainPayouts,
            PendingPayments = pendingPayments,
            PendingRefunds = pendingRefunds
        };

        return Result<AdminPaymentSummaryMetricsDto>.Success(result);
    }

    public async Task<Result<PagedResult<AdminPaymentListItemDto>>> GetAdminPaymentsAsync(GetAdminPaymentsQuery query, CancellationToken cancellationToken = default)
    {
        var dbQuery = _context.Payments
            .Include(p => p.Reservation)
                .ThenInclude(r => r.Customer)
            .Include(p => p.Reservation)
                .ThenInclude(r => r.Boat)
                .ThenInclude(b => b.Captain)
                .ThenInclude(c => c.User)
            .Include(p => p.Reservation)
                .ThenInclude(r => r.TourPackage)
            .Include(p => p.Reservation)
                .ThenInclude(r => r.Boat)
                .ThenInclude(b => b.Location)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            dbQuery = dbQuery.Where(p => 
                p.PaymentNo.Contains(query.Search) ||
                (p.ProviderTransactionId != null && p.ProviderTransactionId.Contains(query.Search)) ||
                p.Reservation.ReservationNo.Contains(query.Search) ||
                p.Reservation.Customer.FirstName.Contains(query.Search) ||
                p.Reservation.Customer.LastName.Contains(query.Search) ||
                p.Reservation.Boat.Name.Contains(query.Search)
            );
        }

        if (!string.IsNullOrWhiteSpace(query.PaymentStatus) && Enum.TryParse<PaymentStatus>(query.PaymentStatus, true, out var paymentStatus))
        {
            dbQuery = dbQuery.Where(p => p.Status == paymentStatus);
        }

        if (!string.IsNullOrWhiteSpace(query.PayoutStatus) && Enum.TryParse<PayoutStatus>(query.PayoutStatus, true, out var payoutStatus))
        {
            dbQuery = dbQuery.Where(p => p.PayoutStatus == payoutStatus);
        }

        if (!string.IsNullOrWhiteSpace(query.RefundStatus) && Enum.TryParse<RefundStatus>(query.RefundStatus, true, out var refundStatus))
        {
            dbQuery = dbQuery.Where(p => p.RefundStatus == refundStatus);
        }
        
        if (!string.IsNullOrWhiteSpace(query.PaymentProvider) && Enum.TryParse<PaymentProvider>(query.PaymentProvider, true, out var provider))
        {
            dbQuery = dbQuery.Where(p => p.PaymentProvider == provider);
        }

        if (query.DateFrom.HasValue)
        {
            dbQuery = dbQuery.Where(p => p.CreatedAt >= query.DateFrom.Value);
        }

        if (query.DateTo.HasValue)
        {
            dbQuery = dbQuery.Where(p => p.CreatedAt <= query.DateTo.Value);
        }

        if (query.CustomerId.HasValue)
        {
            dbQuery = dbQuery.Where(p => p.Reservation.CustomerId == query.CustomerId.Value);
        }

        if (query.CaptainId.HasValue)
        {
            dbQuery = dbQuery.Where(p => p.Reservation.Boat.CaptainId == query.CaptainId.Value);
        }

        if (query.BoatId.HasValue)
        {
            dbQuery = dbQuery.Where(p => p.Reservation.BoatId == query.BoatId.Value);
        }

        var totalCount = await dbQuery.CountAsync(cancellationToken);

        // Sorting
        dbQuery = query.SortBy?.ToLower() switch
        {
            "amount" => query.SortDirection?.ToLower() == "asc" ? dbQuery.OrderBy(p => p.Amount) : dbQuery.OrderByDescending(p => p.Amount),
            "date" => query.SortDirection?.ToLower() == "asc" ? dbQuery.OrderBy(p => p.CreatedAt) : dbQuery.OrderByDescending(p => p.CreatedAt),
            _ => dbQuery.OrderByDescending(p => p.CreatedAt)
        };

        var items = await dbQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new AdminPaymentListItemDto
            {
                Id = p.Id,
                PaymentNo = p.PaymentNo,
                TransactionId = p.ProviderTransactionId,
                ReservationId = p.ReservationId,
                ReservationNo = p.Reservation.ReservationNo,
                CustomerId = p.Reservation.CustomerId,
                CustomerName = p.Reservation.Customer.FirstName + " " + p.Reservation.Customer.LastName,
                CustomerEmail = p.Reservation.Customer.Email,
                CaptainId = p.Reservation.Boat.CaptainId,
                CaptainName = p.Reservation.Boat.Captain.User.FirstName + " " + p.Reservation.Boat.Captain.User.LastName,
                BoatId = p.Reservation.BoatId,
                BoatName = p.Reservation.Boat.Name,
                PackageId = p.Reservation.TourPackageId,
                PackageName = p.Reservation.TourPackage.Name,
                Location = p.Reservation.Boat.Location.Name,
                TourDate = p.Reservation.StartDateTime,
                TotalAmount = p.Amount,
                DepositAmount = p.DepositAmount,
                RemainingAmount = p.RemainingAmount,
                PlatformCommissionAmount = p.PlatformCommission,
                ServiceFeeAmount = p.ServiceFeeAmount,
                CaptainPayoutAmount = p.CaptainEarnings,
                Currency = p.Currency,
                PaymentProvider = p.PaymentProvider.ToString().ToLower(),
                ProviderReferenceNo = p.ProviderReferenceNo,
                PaymentMethod = "CreditCard", // Can be dynamic
                PaymentStatus = p.Status.ToString().ToLower(),
                PayoutStatus = p.PayoutStatus.ToString().ToLower(),
                RefundStatus = p.RefundStatus.ToString().ToLower(),
                CreatedAt = p.CreatedAt,
                PaidAt = p.PaidAt
            })
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<AdminPaymentListItemDto>(items, totalCount, query.Page, query.PageSize);
        return Result<PagedResult<AdminPaymentListItemDto>>.Success(pagedResult);
    }

    public async Task<Result<AdminPaymentDetailDto>> GetAdminPaymentDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var payment = await _context.Payments
            .Include(p => p.Reservation)
                .ThenInclude(r => r.Customer)
            .Include(p => p.Reservation)
                .ThenInclude(r => r.Boat)
                .ThenInclude(b => b.Captain)
                .ThenInclude(c => c.User)
            .Include(p => p.Reservation)
                .ThenInclude(r => r.Boat)
                .ThenInclude(b => b.Captain)
                .ThenInclude(c => c.Company)
            .Include(p => p.Reservation)
                .ThenInclude(r => r.TourPackage)
            .Include(p => p.Reservation)
                .ThenInclude(r => r.Boat)
                .ThenInclude(b => b.Location)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (payment == null)
        {
            return Result<AdminPaymentDetailDto>.Failure("PaymentNotFound", "Payment not found.");
        }

        var dto = new AdminPaymentDetailDto
        {
            Id = payment.Id,
            PaymentNo = payment.PaymentNo,
            TransactionId = payment.ProviderTransactionId,
            ReservationId = payment.ReservationId,
            ReservationNo = payment.Reservation.ReservationNo,
            CustomerId = payment.Reservation.CustomerId,
            CustomerName = payment.Reservation.Customer.FirstName + " " + payment.Reservation.Customer.LastName,
            CustomerEmail = payment.Reservation.Customer.Email,
            CustomerPhoneMasked = payment.Reservation.Customer.PhoneNumber, // Mask in real case if needed
            CaptainId = payment.Reservation.Boat.CaptainId,
            CaptainName = payment.Reservation.Boat.Captain.User.FirstName + " " + payment.Reservation.Boat.Captain.User.LastName,
            CaptainCompanyName = payment.Reservation.Boat.Captain.Company?.CompanyName,
            BoatId = payment.Reservation.BoatId,
            BoatName = payment.Reservation.Boat.Name,
            PackageId = payment.Reservation.TourPackageId,
            PackageName = payment.Reservation.TourPackage.Name,
            Location = payment.Reservation.Boat.Location.Name,
            TourDate = payment.Reservation.StartDateTime,
            GrossTourAmount = payment.GrossTourAmount,
            ServiceFeeAmount = payment.ServiceFeeAmount,
            DiscountAmount = 0, // Mocked for now, need field
            TotalAmount = payment.Amount,
            DepositAmount = payment.DepositAmount,
            RemainingAmount = payment.RemainingAmount,
            PlatformCommissionAmount = payment.PlatformCommission,
            CaptainPayoutAmount = payment.CaptainEarnings,
            RefundedAmount = payment.RefundedAmount,
            NetPlatformRevenue = payment.PlatformCommission + payment.ServiceFeeAmount,
            Currency = payment.Currency,
            PaymentProvider = payment.PaymentProvider.ToString().ToLower(),
            ProviderReferenceNo = payment.ProviderReferenceNo,
            ProviderTransactionId = payment.ProviderTransactionId,
            PaymentStatus = payment.Status.ToString().ToLower(),
            PayoutStatus = payment.PayoutStatus.ToString().ToLower(),
            RefundStatus = payment.RefundStatus.ToString().ToLower(),
            CreatedAt = payment.CreatedAt,
            PaidAt = payment.PaidAt,
            UpdatedAt = payment.UpdatedAt
        };

        // If PayoutInfo is requested
        dto.PayoutInfo = new PaymentPayoutInfoDto
        {
            Id = Guid.NewGuid(), // Mock for now or relation
            CaptainId = dto.CaptainId,
            CaptainName = dto.CaptainName,
            PayoutStatus = payment.PayoutStatus.ToString().ToLower(),
            GrossTourAmount = payment.GrossTourAmount,
            PlatformCommissionAmount = payment.PlatformCommission,
            CaptainPayoutAmount = payment.CaptainEarnings,
            NetPayableAmount = payment.CaptainEarnings
        };

        return Result<AdminPaymentDetailDto>.Success(dto);
    }

    public async Task<Result> ProcessRefundRequestAsync(Guid id, string action, string reason, CancellationToken cancellationToken = default)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (payment == null) return Result.Failure("PaymentNotFound", "Payment not found.");

        if (action.Equals("approve", StringComparison.OrdinalIgnoreCase))
        {
            payment.RefundStatus = RefundStatus.Refunded;
            payment.RefundedAmount = payment.Amount; // Full refund as example
            payment.UpdatedAt = DateTime.UtcNow;
            
            // Add audit/note here
        }
        else if (action.Equals("reject", StringComparison.OrdinalIgnoreCase))
        {
            payment.RefundStatus = RefundStatus.Rejected;
            payment.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> UpdatePayoutStatusAsync(Guid id, string status, string? reason, CancellationToken cancellationToken = default)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (payment == null) return Result.Failure("PaymentNotFound", "Payment not found.");

        if (Enum.TryParse<PayoutStatus>(status, true, out var payoutStatus))
        {
            payment.PayoutStatus = payoutStatus;
            payment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        return Result.Failure("InvalidStatus", "Invalid payout status.");
    }
}
