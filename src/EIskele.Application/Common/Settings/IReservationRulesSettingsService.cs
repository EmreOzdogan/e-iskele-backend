using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Common.Settings;

public interface IReservationRulesSettingsService
{
    Task<Result<ReservationRulesSettingsDto>> GetReservationRulesSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateReservationRulesSettingsAsync(ReservationRulesSettingsDto dto, Guid currentUserId, CancellationToken cancellationToken = default);
}
