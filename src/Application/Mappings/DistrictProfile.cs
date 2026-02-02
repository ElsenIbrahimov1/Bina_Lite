using Application.DTOs.District;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class DistrictProfile : Profile
{
    public DistrictProfile()
    {
        CreateMap<CreateDistrictRequest, District>()
            .ForMember(d => d.Name, o => o.MapFrom(s => (s.Name ?? string.Empty).Trim()))
            .ForMember(d => d.CityId, o => o.MapFrom(s => s.CityId));

        CreateMap<UpdateDistrictRequest, District>()
            .ForMember(d => d.Name, o => o.MapFrom(s => (s.Name ?? string.Empty).Trim()))
            .ForMember(d => d.CityId, o => o.MapFrom(s => s.CityId))
            .ForMember(d => d.Id, o => o.Ignore());

        CreateMap<District, GetAllDistrictResponse>();
        CreateMap<District, GetByIdDistrictResponse>();
    }
}