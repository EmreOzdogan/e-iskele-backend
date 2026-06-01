using System.Collections.Generic;

namespace EIskele.Shared.Responses;

public class ApiErrorResponse
{
    public bool Success { get; set; } = false;
    public string ErrorCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public IEnumerable<ValidationError> Errors { get; set; } = System.Array.Empty<ValidationError>();
    public string TraceId { get; set; } = string.Empty;
}

public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
