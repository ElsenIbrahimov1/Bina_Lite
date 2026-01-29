namespace Application.DTOs.PropertyAdMedia;

public sealed class PropertyMediaDto
{
    public int Id { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public int Order { get; set; }
}