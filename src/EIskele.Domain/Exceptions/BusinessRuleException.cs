using System;

namespace EIskele.Domain.Exceptions;

public class BusinessRuleException : Exception
{
    public string ErrorCode { get; }
    
    public BusinessRuleException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}
