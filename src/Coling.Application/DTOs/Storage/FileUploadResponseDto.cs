namespace Coling.Application.DTOs.Storage;

public class FileUploadResponseDto
{
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long FileSize { get; set; }
}
