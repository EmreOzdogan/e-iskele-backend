using System;
using System.Security.Claims;
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
    private readonly EIskele.Persistence.EIskeleDbContext _dbContext;

    public FilesController(IFileStorageService fileStorageService, EIskele.Persistence.EIskeleDbContext dbContext)
    {
        _fileStorageService = fileStorageService;
        _dbContext = dbContext;
    }

    [HttpPost("upload")]
    [AllowAnonymous]
    public async Task<IActionResult> Upload(IFormFile file, [FromForm] string fileType, [FromForm] bool isPublic = false, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse.CreateFailure("INVALID_FILE", "Dosya seçilmedi veya boş."));
        }

        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid.TryParse(userIdString, out var userId); // Defaults to Guid.Empty if not logged in

        var request = new FileUploadRequest
        {
            Content = file.OpenReadStream(),
            OriginalFileName = file.FileName,
            ContentType = file.ContentType,
            OwnerUserId = userId,
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

        // Upload başarılıysa ApiResponse ile dönüyoruz
        // result.StoredFileName is the unique ID created by LocalFileStorageService, we can use it to fetch the file later.
        return Ok(ApiResponse<FileUploadResult>.CreateSuccess(result));
    }

    [HttpGet("{id:guid}/download")]
    [AllowAnonymous]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken = default)
    {
        var file = await _dbContext.Set<EIskele.Domain.Entities.StoredFile>().FindAsync(new object[] { id }, cancellationToken);
        if (file == null || file.IsDeleted)
        {
            return NotFound(ApiResponse.CreateFailure("FILE_NOT_FOUND", "Dosya bulunamadı veya silinmiş."));
        }

        if (!file.IsPublic)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Unauthorized(ApiResponse.CreateFailure("UNAUTHORIZED", "Bu dosyayı görüntülemek için giriş yapmalısınız."));
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdString, out var userId);

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            if (!isAdmin && file.OwnerUserId != userId)
            {
                return StatusCode(403, ApiResponse.CreateFailure("FORBIDDEN", "Bu dosyayı görüntüleme yetkiniz yok."));
            }
        }

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

            // MimeType'ı yoksa fallback olarak application/octet-stream verelim
            var mimeType = !string.IsNullOrEmpty(file.MimeType) ? file.MimeType : "application/octet-stream";
            
            // PDF veya görseller tarayıcıda açılsın, diğerleri direkt insin istiyorsanız Content-Disposition ayarlayabilirsiniz:
            // Fakat ASP.NET Core 'File' metodu OriginalFileName verilirse otomatik attachment (indirme) yapar. 
            // Biz önizleme yapacağımız için fileName parametresini vermiyoruz (inline göstermek için).
            
            return File(stream, mimeType);
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, ApiResponse.CreateFailure("DOWNLOAD_ERROR", "Dosya indirilirken hata oluştu: " + ex.Message));
        }
    }
}
