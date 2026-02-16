using Domain.Entities.Common;
using Domain.Enums;

namespace Domain.Entities;

public class PropertyAd : BaseEntity<int>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<PropertyMedia> Media { get; set; } = [];

    public string CreatedByUserId { get; set; } = string.Empty;
    public AppUser CreatedByUser { get; set; } = null!;

    public bool IsMortgage { get; set; }

    public bool IsExtract { get; set; }

    public decimal Price { get; set; }
    public int RoomCount { get; set; }
    public decimal AreaInSquareMeters { get; set; }
    public OfferType OfferType { get; set; }

    public PropertyCategory PropertyCategory { get; set; }
}
