using Microsoft.AspNetCore.Http;

namespace Application.DTOs.PropertyAdMedia;

public sealed class PropertyMediaUploadRequest
{
    public IFormFile File { get; set; } = null!;
    public int Order { get; set; }
}