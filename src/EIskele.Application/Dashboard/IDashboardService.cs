using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Dashboard;

public interface IDashboardService
{
    Task<Result<AdminDashboardSummaryResponseDto>> GetAdminDashboardSummaryAsync(CancellationToken cancellationToken = default);
}
