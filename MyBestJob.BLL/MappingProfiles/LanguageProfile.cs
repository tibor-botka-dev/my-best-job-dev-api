using AutoMapper;
using MyBestJob.BLL.ViewModels;
using MyBestJob.DAL.Database.Models;

namespace MyBestJob.BLL.MappingProfiles;

public class LanguageProfile : Profile
{
    public LanguageProfile()
    {
        CreateMap<Language, LanguageViewModel>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
            .ForMember(dest => dest.ExtendedName, opt => opt.MapFrom(x => x.ExtendedName));
    }
}
