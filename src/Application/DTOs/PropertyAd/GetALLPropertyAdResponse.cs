using Domain.Entities;
using Domain.Enums;

namespace Application.DTOs.PropertyAd;

public class GetALLPropertyAdResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

}
