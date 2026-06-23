using EIskele.Application.Common.Results;
using EIskele.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected Guid UserId 
    {
        get
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
        }
    }

    protected IActionResult HandleResult(Result result, string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            return Ok(successMessage == null ? ApiResponse.CreateSuccess() : ApiResponse.CreateSuccess(successMessage));
        }

        return result.Error.Code switch
        {
            "NOT_FOUND" => NotFound(ApiResponse.CreateFailure(result.Error.Code, result.Error.Message)),
            "CONFLICT" => Conflict(ApiResponse.CreateFailure(result.Error.Code, result.Error.Message)),
            "UNAUTHORIZED" => Unauthorized(ApiResponse.CreateFailure(result.Error.Code, result.Error.Message)),
            "FORBIDDEN" => StatusCode(403, ApiResponse.CreateFailure(result.Error.Code, result.Error.Message)),
            _ => BadRequest(ApiResponse.CreateFailure(result.Error.Code, result.Error.Message))
        };
    }

    protected IActionResult HandleResult<T>(Result<T> result, string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            return Ok(successMessage == null ? ApiResponse<T>.CreateSuccess(result.Value) : ApiResponse<T>.CreateSuccess(result.Value, successMessage));
        }

        return result.Error.Code switch
        {
            "NOT_FOUND" => NotFound(ApiResponse<T>.CreateFailure(result.Error.Code, result.Error.Message)),
            "CONFLICT" => Conflict(ApiResponse<T>.CreateFailure(result.Error.Code, result.Error.Message)),
            "UNAUTHORIZED" => Unauthorized(ApiResponse<T>.CreateFailure(result.Error.Code, result.Error.Message)),
            "FORBIDDEN" => StatusCode(403, ApiResponse<T>.CreateFailure(result.Error.Code, result.Error.Message)),
            _ => BadRequest(ApiResponse<T>.CreateFailure(result.Error.Code, result.Error.Message))
        };
    }
}
