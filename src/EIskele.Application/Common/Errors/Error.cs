namespace EIskele.Application.Common.Errors;

public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("ERROR.NULL_VALUE", "Değer boş olamaz.");
}
