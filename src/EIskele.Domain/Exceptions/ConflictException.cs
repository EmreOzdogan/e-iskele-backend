using System;

namespace EIskele.Domain.Exceptions;

public class ConflictException : Exception
{
    public string ErrorCode { get; }

    public ConflictException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}
