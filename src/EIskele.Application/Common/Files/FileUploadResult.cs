namespace EIskele.Application.Common.Files;

public class FileUploadResult
{
    public bool Success { get; set; }
    public Guid FileId { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string PublicUrl { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public string StorageProvider { get; set; } = string.Empty;
}
