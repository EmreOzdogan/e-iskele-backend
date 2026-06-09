using EIskele.Domain.Enums;

namespace EIskele.Application.Users.DTOs;

public class AdminUserListQuery
{
    public string? Search { get; set; }
    public string? Role { get; set; }
    public UserStatus? Status { get; set; }
    public DateTime? CreatedDateStart { get; set; }
    public DateTime? CreatedDateEnd { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
}
