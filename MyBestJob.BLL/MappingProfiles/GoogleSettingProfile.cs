using AutoMapper;
using MyBestJob.BLL.ViewModels;
using MyBestJob.DAL.Constants;
using MyBestJob.DAL.Database.Models;

namespace MyBestJob.BLL.MappingProfiles;

public class GoogleSettingProfile : Profile
{
    public GoogleSettingProfile()
    {
        CreateMap<GoogleSetting, GoogleUrlViewModel>()
            .ForMember(dest => dest.ClientId, opt => opt.MapFrom(x => x.ClientId))
            .ForMember(dest => dest.AccessType, opt => opt.MapFrom(x => Constants.Google.AccessType))
            .ForMember(dest => dest.ResponseType, opt => opt.MapFrom(x => Constants.Google.ResponseType))
            .ForMember(dest => dest.Prompt, opt => opt.MapFrom(x => Constants.Google.Prompt))
            .ForMember(dest => dest.Scope, opt => opt.MapFrom(x => x.Scope))
            .ForMember(dest => dest.State, opt => opt.MapFrom(x => Constants.Google.State));
    }
}
