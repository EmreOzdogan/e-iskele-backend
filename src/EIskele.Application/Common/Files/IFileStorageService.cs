using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EIskele.Application.Common.Files;

public interface IFileStorageService
{
    Task<FileUploadResult> UploadAsync(FileUploadRequest request, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(string storagePath, CancellationToken cancellationToken = default);
    Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default);
}
