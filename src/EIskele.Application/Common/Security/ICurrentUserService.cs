using System;

namespace EIskele.Application.Common.Security;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Role { get; }
}
