using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Files;
using EIskele.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers;

[Route("api/[controller]")]
public class FilesController : BaseController
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IStoredFileService _storedFileService;

    public FilesController(IFileStorageService fileStorageService, IStoredFileService storedFileService)
    {
        _fileStorageService = fileStorageService;
        _storedFileService = storedFileService;
    }

    [HttpPost("upload")]
    [AllowAnonymous]
    public async Task<IActionResult> Upload(IFormFile file, [FromForm] string fileType, [FromForm] bool isPublic = false, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse.CreateFailure("INVALID_FILE", "Dosya seçilmedi veya boş."));
        }

        var request = new FileUploadRequest
        {
            Content = file.OpenReadStream(),
            OriginalFileName = file.FileName,
            ContentType = file.ContentType,
            OwnerUserId = this.UserId,
            RelatedEntityType = "CaptainApplication", // Varsayılan olarak başvuru diyelim, isterseniz parametre alabiliriz
            RelatedEntityId = string.Empty, // Başvuru ID'si henüz oluşmadığı için boş kalabilir
            FileType = fileType,
            IsPublic = isPublic
        };

        var result = await _fileStorageService.UploadAsync(request, cancellationToken);
        if (!result.Success)
        {
            return BadRequest(ApiResponse.CreateFailure("UPLOAD_FAILED", "Dosya yüklenirken bir hata oluştu."));
        }

        return Ok(ApiResponse<FileUploadResult>.CreateSuccess(result));
    }

    [HttpGet("{id:guid}/download")]
    [AllowAnonymous]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken = default)
    {
        var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
        var fileResult = await _storedFileService.GetFileRecordAsync(id, this.UserId, isAdmin, cancellationToken);

        if (!fileResult.IsSuccess)
        {
            return HandleResult(fileResult);
        }

        var file = fileResult.Value;

        if (string.IsNullOrEmpty(file.StoragePath))
        {
            return NotFound(ApiResponse.CreateFailure("STORAGE_NOT_FOUND", "Fiziksel dosya bulunamadı."));
        }

        try
        {
            var stream = await _fileStorageService.DownloadAsync(file.StoragePath, cancellationToken);
            if (stream == null)
            {
                return NotFound(ApiResponse.CreateFailure("FILE_MISSING", "Dosya sunucuda bulunamadı."));
            }

            var mimeType = !string.IsNullOrEmpty(file.MimeType) ? file.MimeType : "application/octet-stream";
            return File(stream, mimeType);
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, ApiResponse.CreateFailure("DOWNLOAD_ERROR", "Dosya indirilirken hata oluştu: " + ex.Message));
        }
    }
}
