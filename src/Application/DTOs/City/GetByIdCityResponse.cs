using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.City;

public class GetByIdCityResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<CityDistrictDto> Districts { get; set; } = [];
}