using Application.DTOs.City;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class CityProfile : Profile
{
    public CityProfile()
    {
        CreateMap<CreateCityRequest, City>()
            .ForMember(d => d.Name, o => o.MapFrom(s => (s.Name ?? string.Empty).Trim()));

        // ✅ District mapping for nested list
        CreateMap<District, CityDistrictDto>();

        CreateMap<City, GetAllCityResponse>();
        CreateMap<City, GetByIdCityResponse>();

        CreateMap<UpdateCityRequest, City>()
            .ForMember(d => d.Name, o => o.MapFrom(s => (s.Name ?? string.Empty).Trim()))
            .ForMember(d => d.Id, o => o.Ignore());
    }
}