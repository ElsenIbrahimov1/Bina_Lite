namespace Application.DTOs.District;

public class CreateDistrictRequest
{
    public string Name { get; set; } = string.Empty;
    public int CityId { get; set; }
}