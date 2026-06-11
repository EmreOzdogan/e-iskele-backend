namespace EIskele.Application.Users.DTOs;

public class CreateAdminUserRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool GenerateTempPassword { get; set; }
    public bool RequirePasswordChange { get; set; }
}
