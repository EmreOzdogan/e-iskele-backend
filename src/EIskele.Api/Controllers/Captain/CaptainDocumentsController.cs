using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Captains;
using EIskele.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers.Captain;

[Route("api/captain/documents")]
[Authorize(Roles = "Captain")] // Kaptan rolü gerekli
public class CaptainDocumentsController : BaseController
{
    private readonly ICaptainDocumentsService _documentsService;

    public CaptainDocumentsController(ICaptainDocumentsService documentsService)
    {
        _documentsService = documentsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDocuments(CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized(ApiResponse.CreateFailure("UNAUTHORIZED", "Kullanıcı doğrulanamadı."));

        var result = await _documentsService.GetCaptainDocumentsAsync(userId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{documentId}/upload")]
    public async Task<IActionResult> UploadDocument(string documentId, IFormFile file, [FromForm] UploadCaptainDocumentRequest request, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse.CreateFailure("INVALID_FILE", "Dosya boş olamaz."));

        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized(ApiResponse.CreateFailure("UNAUTHORIZED", "Kullanıcı doğrulanamadı."));

        var result = await _documentsService.UploadDocumentAsync(userId, documentId, file.OpenReadStream(), file.FileName, file.ContentType, request, cancellationToken);
        return HandleResult(result);
    }
}
