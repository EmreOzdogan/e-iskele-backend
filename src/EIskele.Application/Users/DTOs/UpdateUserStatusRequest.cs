using EIskele.Domain.Enums;

namespace EIskele.Application.Users.DTOs;

public class UpdateUserStatusRequest
{
    public UserStatus Status { get; set; }
    public string? Reason { get; set; }
}
