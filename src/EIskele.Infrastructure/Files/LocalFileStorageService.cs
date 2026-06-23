using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Files;
using EIskele.Domain.Entities;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace EIskele.Infrastructure.Files;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;
    private readonly EIskeleDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalFileStorageService(IWebHostEnvironment env, EIskeleDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _env = env;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<FileUploadResult> UploadAsync(FileUploadRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Content == null || request.Content.Length == 0)
        {
            return new FileUploadResult { Success = false };
        }

        var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{request.OriginalFileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await request.Content.CopyToAsync(fileStream, cancellationToken);
        }

        var storedFile = new StoredFile
        {
            Id = Guid.NewGuid(),
            OwnerUserId = request.OwnerUserId,
            RelatedEntityType = request.RelatedEntityType,
            RelatedEntityId = request.RelatedEntityId,
            FileType = Enum.Parse<StoredFileType>(request.FileType, true),
            OriginalFileName = request.OriginalFileName,
            StoredFileName = uniqueFileName,
            MimeType = request.ContentType,
            Extension = Path.GetExtension(request.OriginalFileName),
            SizeInBytes = request.Content.Length,
            StorageProvider = "Local",
            StoragePath = filePath,
            PublicUrl = $"/uploads/{uniqueFileName}",
            IsPublic = request.IsPublic,
            Status = StoredFileStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.StoredFiles.AddAsync(storedFile, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new FileUploadResult
        {
            Success = true,
            FileId = storedFile.Id,
            StoragePath = filePath,
            PublicUrl = request.IsPublic ? $"/uploads/{uniqueFileName}" : string.Empty,
            StoredFileName = uniqueFileName,
            SizeInBytes = request.Content.Length,
            StorageProvider = "Local"
        };
    }

    public Task<Stream> DownloadAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(storagePath))
        {
            throw new FileNotFoundException("Dosya bulunamadı.", storagePath);
        }

        Stream stream = new FileStream(storagePath, FileMode.Open, FileAccess.Read);
        return Task.FromResult(stream);
    }

    public async Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (File.Exists(storagePath))
        {
            File.Delete(storagePath);
        }

        await Task.CompletedTask;
    }
}
