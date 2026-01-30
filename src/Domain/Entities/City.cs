using Domain.Entities.Common;

namespace Domain.Entities;


public class City : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
}
