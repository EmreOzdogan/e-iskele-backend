using System;

namespace EIskele.Application.Common.Results;

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Errors.Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Hatalı bir işlemin sonucu döndürülemez.");

    public static Result<TValue> Success(TValue value) => new(value, true, Errors.Error.None);
    public static new Result<TValue> Failure(Errors.Error error) => new(default, false, error);
    public static new Result<TValue> Failure(string errorCode, string message) => new(default, false, new Errors.Error(errorCode, message));
}
