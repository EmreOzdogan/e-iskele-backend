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

    public FilesController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
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
}
