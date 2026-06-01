using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using EIskele.Domain.Exceptions;
using EIskele.Shared.Responses;

namespace EIskele.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var traceId = context.TraceIdentifier;
        _logger.LogError(ex, "An unhandled exception has occurred. TraceId: {TraceId}", traceId);

        context.Response.ContentType = "application/json";

        var response = new ApiErrorResponse
        {
            Success = false,
            TraceId = traceId
        };

        switch (ex)
        {
            case BusinessRuleException businessEx:
                context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                response.ErrorCode = businessEx.ErrorCode;
                response.Message = businessEx.Message;
                break;

            case ConflictException conflictEx:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response.ErrorCode = conflictEx.ErrorCode;
                response.Message = conflictEx.Message;
                break;

            case NotFoundException notFoundEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.ErrorCode = notFoundEx.ErrorCode;
                response.Message = notFoundEx.Message;
                break;
                
            case ForbiddenException forbiddenEx:
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                response.ErrorCode = forbiddenEx.ErrorCode;
                response.Message = forbiddenEx.Message;
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ErrorCode = "INTERNAL_SERVER_ERROR";
                response.Message = "Beklenmeyen bir hata oluştu. Lütfen tekrar deneyin.";
                break;
        }

        var result = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(result);
    }
}
