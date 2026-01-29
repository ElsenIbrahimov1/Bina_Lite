using Application.DTOs.PropertyAdMedia;
using Application.DTOs.PropertyAd;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class PropertyAdProfile : Profile
{
    public PropertyAdProfile()
    {
    
        CreateMap<CreatePropertyAdRequest, PropertyAd>()
            .ForMember(d => d.Title, o => o.MapFrom(s => (s.Title ?? string.Empty).Trim()))
            .ForMember(d => d.Description, o => o.MapFrom(s => (s.Description ?? string.Empty).Trim()));

        CreateMap<PropertyAd, GetALLPropertyAdResponse>();
        CreateMap<PropertyAd, GetByIdPropertyAdResponse>();

        CreateMap<PropertyMedia, PropertyMediaDto>();

        CreateMap<UpdatePropertyAdRequest, PropertyAd>()
            .ForMember(d => d.Title, o => o.MapFrom(s => (s.Title ?? string.Empty).Trim()))
            .ForMember(d => d.Description, o => o.MapFrom(s => (s.Description ?? string.Empty).Trim()))
            .ForMember(d => d.Id, o => o.Ignore());
    }
}
