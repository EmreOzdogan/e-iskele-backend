using System;
using System.IO;

namespace EIskele.Application.Common.Files;

public class FileUploadRequest
{
    public Stream Content { get; set; } = Stream.Null;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public Guid OwnerUserId { get; set; }
    public string RelatedEntityType { get; set; } = string.Empty;
    public string RelatedEntityId { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
}
