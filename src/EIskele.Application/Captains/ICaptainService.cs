using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Captains;

public interface ICaptainService
{
    Task<Result<CaptainApplicationResponse>> ApplyAsync(CaptainApplicationRequest request, CancellationToken cancellationToken = default);
    Task<Result> ApproveApplicationAsync(Guid applicationId, CancellationToken cancellationToken = default);
    Task<Result> RejectApplicationAsync(Guid applicationId, string reason, CancellationToken cancellationToken = default);
}
