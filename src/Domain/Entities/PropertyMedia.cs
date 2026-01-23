using Domain.Entities.Common;

namespace Domain.Entities;

public class PropertyMedia: BaseEntity
{
    public int PropertyAdId { get; set; }
    public PropertyAd PropertyAd { get; set; } 
    public string MediaUrl { get; set; }
    public string MediaType { get; set; } 

    public int Order { get; set; }
}
