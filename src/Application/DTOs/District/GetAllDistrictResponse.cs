namespace Application.DTOs.District;

public class GetAllDistrictResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CityId { get; set; }
}
