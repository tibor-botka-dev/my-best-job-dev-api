using Microsoft.AspNetCore.Mvc;
using MyBestJob.DAL.Constants;

namespace MyBestJob.API.Extensions;

public static partial class Extensions
{
    public static Dictionary<string, string> GetApiRoutes(this IUrlHelper urlHelper)
        => new()
        {
            ["initUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.Init)!,

            ["signInUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.SignIn)!,
            ["signUpUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.SignUp)!,
            ["signOutUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.SignOut)!,
            ["refreshTokenUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.RefreshToken)!,
            ["confirmAccountUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.ConfirmAccount)!,
            ["forgotPasswordUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.ForgotPassword)!,
            ["resetPasswordUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.ResetPassword)!,
            ["changePasswordUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.ChangePassword)!,

            ["userUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.User)!,
            ["getUserUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.GetUser)!,
            ["getUserAvatarUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.GetUserAvatar)!,

            ["settingUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.Setting)!,
            ["getMailSettingUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.GetMailSetting)!,
            ["getEmailTemplatesUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.GetEmailTemplates)!,
            ["updateMailSettingUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.UpdateMailSetting)!,
            ["updateEmailTemplateUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.UpdateEmailTemplate)!,
            ["createEmailTemplateValueUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.CreateEmailTemplateValue)!,
            ["updateEmailTemplateValueUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.UpdateEmailTemplateValue)!,
            ["deleteEmailTemplateValueUrl"] = urlHelper.RouteUrl(Constants.ApiRoutes.DeleteEmailTemplateValue)!,

        };
}
