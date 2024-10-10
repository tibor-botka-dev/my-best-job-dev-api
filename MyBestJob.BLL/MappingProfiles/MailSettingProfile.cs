using AutoMapper;
using MyBestJob.BLL.Stuff;
using MyBestJob.BLL.ViewModels;
using MyBestJob.DAL.Database.Models;

namespace MyBestJob.BLL.MappingProfiles;

public class MailSettingProfile : Profile
{
    public MailSettingProfile()
    {
        CreateMap<MailSetting, GetMailSettingViewModel>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(x => x.Email))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(x => x.DisplayName))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(x => Crypto.DecryptSensitiveData(x.AesPassword, x.AesKey)))
            .ForMember(dest => dest.Host, opt => opt.MapFrom(x => x.Host))
            .ForMember(dest => dest.Port, opt => opt.MapFrom(x => x.Port));

        CreateMap<MailSetting, GetEmailTemplateByTypeViewModel>()
            .ForMember(dest => dest.EmailTemplateValues, opt => opt.MapFrom(x => x.EmailTemplateValues));

        CreateMap<MailSetting, GetEmailTemplatesViewModel>()
            .ForMember(dest => dest.EmailTemplates, opt => opt.MapFrom(x => x.EmailTemplates))
            .ForMember(dest => dest.EmailTemplateValues, opt => opt.MapFrom(x => x.EmailTemplateValues));

        CreateMap<EmailTemplate, GetEmailTemplateViewModel>()
            .ForMember(dest => dest.EmailTemplateType, opt => opt.MapFrom(x => x.EmailTemplateType))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
            .ForMember(dest => dest.Subject, opt => opt.MapFrom(x => x.Subject))
            .ForMember(dest => dest.HtmlTemplate, opt => opt.MapFrom(x => x.HtmlTemplate));

        CreateMap<EmailTemplateValue, GetEmailTemplateValueViewModel>()
            .ForMember(dest => dest.IsRequired, opt => opt.MapFrom(x => x.IsRequired))
            .ForMember(dest => dest.Key, opt => opt.MapFrom(x => x.Key))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
            .ForMember(dest => dest.EmailTemplateValueType, opt => opt.MapFrom(x => x.EmailTemplateValueType))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(x => x.Value));

        CreateMap<EditMailSettingViewModel, MailSetting>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(x => x.Email))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(x => x.DisplayName))
            .ForMember(dest => dest.AesPassword, opt => opt.MapFrom((x, y) => Crypto.EncryptSensitiveData(x.Password, y.AesKey)))
            .ForMember(dest => dest.Host, opt => opt.MapFrom(x => x.Host))
            .ForMember(dest => dest.Port, opt => opt.MapFrom(x => x.Port));

        CreateMap<EditEmailTemplateViewModel, EmailTemplate>()
            .ForMember(dest => dest.Subject, opt => opt.MapFrom((patch, from) => patch.Subject ?? from.Subject))
            .ForMember(dest => dest.HtmlTemplate, opt => opt.MapFrom((patch, from) => patch.HtmlTemplate ?? from.HtmlTemplate));

        CreateMap<CreateEmailTemplateValueViewModel, EmailTemplateValue>()
            .ForMember(dest => dest.IsRequired, opt => opt.MapFrom(x => false))
            .ForMember(dest => dest.Key, opt => opt.MapFrom(x => x.Key.TrimStart('-').TrimEnd('-').Trim()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name.Trim()))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(x => x.Value));

        CreateMap<EditEmailTemplateValueViewModel, EmailTemplateValue>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name.Trim()))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(x => x.Value));
    }
}
