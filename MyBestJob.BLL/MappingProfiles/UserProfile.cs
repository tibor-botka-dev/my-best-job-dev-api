using AutoMapper;
using MyBestJob.BLL.Stuff;
using MyBestJob.BLL.ViewModels;
using MyBestJob.DAL.Database.Models;
using Microsoft.AspNetCore.Http;
using static MyBestJob.DAL.Constants.Constants;

namespace MyBestJob.BLL.MappingProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, GetUserViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(x => x.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(x => x.LastName))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(x => x.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(x => x.Email))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(x => x.Roles))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(x => x.AvatarUrl ?? x.AvatarBase64));

        CreateMap<CreateUserViewModel, User>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(x => x.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(x => x.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(x => x.Email))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(x => x.Email))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(x => true))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(x => x.Roles))
            .ForMember(dest => dest.AvatarBase64, opt => opt.MapFrom(x => GetAvatar(x.Avatar, null)));

        CreateMap<EditUserViewModel, User>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(x => x.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(x => x.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(x => x.Email))
            .ForMember(dest => dest.AvatarBase64, opt => opt.MapFrom((x, y) => GetAvatar(x.Avatar, y.AvatarBase64)));

        CreateMap<SignUpViewModel, User>()
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(x => x.LastName))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(x => x.FirstName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(x => x.Email))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(x => x.Email))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(x => new List<Guid> { DefaultRoles.UserRoleId }));

        CreateMap<ExternalSignUpViewModel, User>()
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(x => x.LastName))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(x => x.FirstName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(x => x.Email))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(x => x.Email))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(x => x.AvatarUrl))
            .ForMember(dest => dest.IsGoogleAccount, opt => opt.MapFrom(x => x.IsGoogleAccount))
            .ForMember(dest => dest.IsFacebookAccount, opt => opt.MapFrom(x => x.IsFacebookAccount))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(x => new List<Guid> { DefaultRoles.UserRoleId }));
    }

    private static string? GetAvatar(IFormFile? avatar, string? oldAvatar = null)
    {
        if (avatar == null)
            return oldAvatar;

        var result = avatar.ValidateAndGetAvatarAsBase64();

        return result;
    }
}
