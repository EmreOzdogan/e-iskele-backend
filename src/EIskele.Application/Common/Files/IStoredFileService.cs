using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Domain.Entities;

namespace EIskele.Application.Common.Files;

public interface IStoredFileService
{
    Task<Result<StoredFile>> GetFileRecordAsync(Guid fileId, Guid currentUserId, bool isAdmin, CancellationToken cancellationToken = default);
    Task<Result> DeleteFileRecordAsync(Guid fileId, Guid currentUserId, bool isAdmin, CancellationToken cancellationToken = default);
}
