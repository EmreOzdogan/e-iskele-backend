using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using System.IO;

namespace EIskele.Application.Captains;

public interface ICaptainDocumentsService
{
    Task<Result<CaptainDocumentsDataDto>> GetCaptainDocumentsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result> UploadDocumentAsync(Guid userId, string documentId, Stream fileStream, string fileName, string contentType, UploadCaptainDocumentRequest request, CancellationToken cancellationToken = default);
}
