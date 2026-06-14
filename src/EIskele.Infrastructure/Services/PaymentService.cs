using EIskele.Application.Payments;
using EIskele.Persistence;

namespace EIskele.Infrastructure.Services;

public partial class PaymentService : IPaymentService
{
    private readonly EIskeleDbContext _context;

    public PaymentService(EIskeleDbContext context)
    {
        _context = context;
    }
}
