using AutoMapper;
using MyBestJob.BLL.ViewModels;
using MyBestJob.DAL.Database.Models;

namespace MyBestJob.BLL.MappingProfiles;

public class IdleSettingProfile : Profile
{
    public IdleSettingProfile()
    {
        CreateMap<IdleSetting, GetIdleSettingViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id))
            .ForMember(dest => dest.CreatorUserId, opt => opt.MapFrom(x => x.CreatedByUserId))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(x => x.Duration))
            .ForMember(dest => dest.Reminder, opt => opt.MapFrom(x => x.Reminder))
            .ForMember(dest => dest.Wait, opt => opt.MapFrom(x => x.Wait))
            .ForMember(dest => dest.InBackground, opt => opt.MapFrom(x => x.InBackground))
            .ForMember(dest => dest.Loop, opt => opt.MapFrom(x => x.Loop))
            .ForMember(dest => dest.TurnedOn, opt => opt.MapFrom(x => x.TurnedOn));

        CreateMap<CreateEditIdleSettingViewModel, IdleSetting>()
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(x => x.Duration))
            .ForMember(dest => dest.Reminder, opt => opt.MapFrom(x => x.Reminder))
            .ForMember(dest => dest.Wait, opt => opt.MapFrom(x => x.Wait))
            .ForMember(dest => dest.InBackground, opt => opt.MapFrom(x => x.InBackground))
            .ForMember(dest => dest.Loop, opt => opt.MapFrom(x => x.Loop))
            .ForMember(dest => dest.TurnedOn, opt => opt.MapFrom(x => x.TurnedOn));
    }
}
