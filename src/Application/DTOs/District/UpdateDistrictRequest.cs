namespace Application.DTOs.District;

public class UpdateDistrictRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CityId { get; set; }
}
