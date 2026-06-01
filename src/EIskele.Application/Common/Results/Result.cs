using System;

namespace EIskele.Application.Common.Results;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Errors.Error Error { get; }

    protected Result(bool isSuccess, Errors.Error error)
    {
        if (isSuccess && error != Errors.Error.None || !isSuccess && error == Errors.Error.None)
        {
            throw new ArgumentException("Geçersiz Result durumu.", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Errors.Error.None);
    public static Result Failure(Errors.Error error) => new(false, error);
    public static Result Failure(string errorCode, string message) => new(false, new Errors.Error(errorCode, message));
}
