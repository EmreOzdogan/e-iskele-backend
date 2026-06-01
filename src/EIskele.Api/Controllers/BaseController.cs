using EIskele.Application.Common.Results;
using EIskele.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace EIskele.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponse.CreateSuccess());
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

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<T>.CreateSuccess(result.Value));
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
