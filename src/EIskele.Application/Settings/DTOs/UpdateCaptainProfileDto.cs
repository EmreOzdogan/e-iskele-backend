namespace EIskele.Application.Settings.DTOs;

public class UpdateCaptainProfileDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ShortBio { get; set; } = string.Empty;
    public string[] Languages { get; set; } = Array.Empty<string>();
}
