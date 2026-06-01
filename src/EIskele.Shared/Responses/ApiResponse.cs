namespace EIskele.Shared.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public object? Errors { get; set; }

    public static ApiResponse<T> CreateSuccess(T data, string message = "İşlem başarılı.")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> CreateFailure(string errorCode, string message, object? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Data = default,
            Message = message,
            ErrorCode = errorCode,
            Errors = errors
        };
    }
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse CreateSuccess(string message = "İşlem başarılı.")
    {
        return new ApiResponse
        {
            Success = true,
            Data = null,
            Message = message
        };
    }

    public static new ApiResponse CreateFailure(string errorCode, string message, object? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Data = null,
            Message = message,
            ErrorCode = errorCode,
            Errors = errors
        };
    }
}
