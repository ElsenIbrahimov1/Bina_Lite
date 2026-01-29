using Application.DTOs.PropertyAdMedia;
using Domain.Enums;

namespace Application.DTOs.PropertyAd;

public class GetByIdPropertyAdResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<PropertyMediaDto> Media { get; set; } = [];

    public bool IsMortgage { get; set; }

    public bool IsExtract { get; set; }

    public decimal Price { get; set; }
    public int RoomCount { get; set; }
    public decimal AreaInSquareMeters { get; set; }
    public OfferType OfferType { get; set; }

    public PropertyCategory PropertyCategory { get; set; }
}
