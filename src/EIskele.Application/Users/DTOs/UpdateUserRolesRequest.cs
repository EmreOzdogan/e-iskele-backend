namespace EIskele.Application.Users.DTOs;

public class UpdateUserRolesRequest
{
    public List<string> Roles { get; set; } = new();
}
