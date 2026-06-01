using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class SystemSetting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string ValueType { get; set; } = "String"; // String, Int, Decimal, Boolean
    public string Group { get; set; } = string.Empty;
    public bool IsSensitive { get; set; } = false;
    public bool IsEditable { get; set; } = true;
    public string? Description { get; set; }
}
