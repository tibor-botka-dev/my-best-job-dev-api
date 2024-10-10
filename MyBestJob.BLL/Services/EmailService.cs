using MyBestJob.BLL.Stuff;
using MyBestJob.BLL.ViewModels;
using MyBestJob.DAL.Constants;
using MyBestJob.DAL.Database.Models;
using MyBestJob.DAL.Enums;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace MyBestJob.BLL.Services;

public interface IEmailService
{
    Task SendEmail(EmailTemplateType emailTemplateType, User user);
}

public class EmailService(UserManager<User> userManager,
    ISettingService settingService,
    IOptions<RouteSetting> routeSetting,
    IOptions<MailSetting> mailSetting) : IEmailService
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly ISettingService _settingService = settingService;
    private readonly RouteSetting _routeSetting = routeSetting.Value;
    private readonly MailSetting _mailSetting = mailSetting.Value;

    public async Task SendEmail(EmailTemplateType emailTemplateType, User user)
    {
        var emailTemplateByType = await _settingService.GetEmailTemplateByType(emailTemplateType);

        await ReplaceHtmlTemplate(emailTemplateByType.EmailTemplateValues, emailTemplateByType.EmailTemplate, user);

        await SendEmail(emailTemplateByType.EmailTemplate, user);
    }

    private async Task ReplaceHtmlTemplate(List<GetEmailTemplateValueViewModel> emailTemplateValues,
        GetEmailTemplateViewModel emailTemplate,
        User user)
    {
        foreach (var emailTemplateValue in emailTemplateValues)
        {
            if (!emailTemplate.HtmlTemplate.Contains($"--{emailTemplateValue.Key}--"))
                continue;

            if (!emailTemplateValue.IsRequired && !string.IsNullOrEmpty(emailTemplateValue.Value))
            {
                emailTemplate.HtmlTemplate = emailTemplate.HtmlTemplate.ReplaceHtmlTemplate(emailTemplateValue.Key, emailTemplateValue.Value);
                continue;
            }

            var value = emailTemplateValue.EmailTemplateValueType switch
            {
                EmailTemplateValueType.HomePageUrl => _routeSetting.Routes.GetFrontEndCallbackUrl(RouteType.Home),
                EmailTemplateValueType.AdminEmailAddress => _mailSetting.Email,
                EmailTemplateValueType.UserName => user.FullName,
                EmailTemplateValueType.ResetPasswordUrl => _routeSetting.Routes.GetFrontEndCallbackUrl(RouteType.ResetPassword, await _userManager.GeneratePasswordResetTokenAsync(user)),
                EmailTemplateValueType.ConfirmAccountUrl => _routeSetting.Routes.GetFrontEndCallbackUrl(RouteType.ConfirmAccount, user.Email!, await _userManager.GenerateEmailConfirmationTokenAsync(user)),
                EmailTemplateValueType.AppName => Constants.ApplicationName,
                _ => string.Empty
            };

            emailTemplate.HtmlTemplate = emailTemplate.HtmlTemplate.ReplaceHtmlTemplate(emailTemplateValue.Key, value);
        }
    }

    private async Task SendEmail(GetEmailTemplateViewModel viewModel, User user)
    {
        var mailSetting = await _settingService.GetMailSetting();

        var email = new MimeMessage
        {
            Subject = viewModel.Subject,
            Body = new TextPart(TextFormat.Html)
            {
                Text = viewModel.HtmlTemplate
            }
        };
        email.From.Add(new MailboxAddress(mailSetting.DisplayName, mailSetting.Email));
        email.To.Add(new MailboxAddress(user.FullName, user.Email));

        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync(mailSetting.Host, mailSetting.Port, SecureSocketOptions.StartTls);
        await smtpClient.AuthenticateAsync(mailSetting.Email, mailSetting.Password);
        await smtpClient.SendAsync(email);
        await smtpClient.DisconnectAsync(true);
    }
}