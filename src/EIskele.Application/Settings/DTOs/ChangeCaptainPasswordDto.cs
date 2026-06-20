namespace EIskele.Application.Settings.DTOs;

public class ChangeCaptainPasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
