using Application.DTOs.PropertyAd;
using AutoMapper;
using Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        CreateMap<UpdatePropertyAdRequest, PropertyAd>()
            .ForMember(d => d.Title, o => o.MapFrom(s => (s.Title ?? string.Empty).Trim()))
            .ForMember(d => d.Description, o => o.MapFrom(s => (s.Description ?? string.Empty).Trim()))
            .ForMember(d => d.Id, o => o.Ignore());
    }
}
