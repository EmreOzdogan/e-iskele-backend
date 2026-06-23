using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Common.Settings;

public interface IPaymentSettingsService
{
    Task<Result<PaymentSettingsDto>> GetPaymentSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdatePaymentSettingsAsync(PaymentSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);
}
