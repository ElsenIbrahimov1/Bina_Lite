using Domain.Entities.Common;

namespace Domain.Entities;

public class PropertyMedia : BaseEntity<int>
{
    public int PropertyAdId { get; set; }
    public PropertyAd PropertyAd { get; set; } = null!;

    public string ObjectKey { get; set; } = string.Empty;

    public int Order { get; set; }
}
