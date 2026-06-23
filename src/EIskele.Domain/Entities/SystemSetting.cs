using EIskele.Domain.Common;
using EIskele.Domain.Enums;

namespace EIskele.Domain.Entities;

public class SystemSetting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public SystemSettingValueType ValueType { get; set; } = SystemSettingValueType.String;
    public string Group { get; set; } = string.Empty;
    public bool IsSensitive { get; set; } = false;
    public bool IsEditable { get; set; } = true;
    public string? Description { get; set; }
}
