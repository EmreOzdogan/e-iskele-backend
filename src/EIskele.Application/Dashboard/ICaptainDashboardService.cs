using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Dashboard;

public interface ICaptainDashboardService
{
    Task<Result<CaptainDashboardDataDto>> GetDashboardSummaryAsync(string userId, CancellationToken cancellationToken = default);
}
