using System;

namespace EIskele.Domain.Exceptions;

public class ForbiddenException : Exception
{
    public string ErrorCode { get; }

    public ForbiddenException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}
