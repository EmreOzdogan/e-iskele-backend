using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Layout;

public interface ICaptainLayoutService
{
    Task<Result<CaptainLayoutDataDto>> GetLayoutDataAsync(string userId, CancellationToken cancellationToken = default);
}
