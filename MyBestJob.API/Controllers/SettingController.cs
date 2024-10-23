using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MyBestJob.BLL.Attributes;
using MyBestJob.BLL.Exceptions;
using MyBestJob.BLL.Services;
using MyBestJob.BLL.Stuff;
using MyBestJob.BLL.ViewModels;
using MyBestJob.DAL.Database.Models;
using MyBestJob.DAL.Enums;
using static MyBestJob.DAL.Constants.Constants;

namespace MyBestJob.API.Controllers;

[ApiController, Route("api/setting", Name = ApiRoutes.Setting)]
[AuthorizeByRole(nameof(DAL.Enums.Role.Administrator))]
public class SettingController(ILogger<SettingController> logger,
    IStringLocalizer<SettingController> localizer,
    ISettingService settingService) : ControllerBase
{
    private readonly ILogger<SettingController> _logger = logger;
    private readonly IStringLocalizer L = localizer;

    private readonly ISettingService _settingService = settingService;

    [HttpGet, Route("get-mail-setting", Name = ApiRoutes.GetMailSetting)]
    public async Task<IActionResult> GetMailSetting()
    {
        try
        {
            var mailSetting = await _settingService.GetMailSetting();

            return Ok(new
            {
                mailSetting
            });
        }
        catch (MissingSettingException ex)
        {
            _logger.LogWarning(ex, "Mail settings not found.");
            return Conflict(L["Mail beállítások nem találhatóak"].Value);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during get mail setting data.");
            return BadRequest(L["Mail beállítások lekérdezése nem sikerült"].Value);
        }
    }

    [HttpGet, Route("get-email-templates", Name = ApiRoutes.GetEmailTemplates)]
    public async Task<IActionResult> GetEmailTemplates()
    {
        try
        {
            var emailTemplates = await _settingService.GetEmailTemplates();

            return Ok(new
            {
                emailTemplates
            });
        }
        catch (MissingSettingException ex)
        {
            _logger.LogWarning(ex, "Mail settings not found.");
            return Conflict(L["Mail beállítások nem találhatóak"].Value);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during get email templates.");
            return BadRequest(L["Email sablonok lekérdezése nem sikerült"].Value);
        }
    }

    [HttpPut, Route("update-mail-setting", Name = ApiRoutes.UpdateMailSetting)]
    public async Task<IActionResult> UpdateMailSetting(EditMailSettingViewModel viewModel)
    {
        try
        {
            var userId = await User.Claims.GetRequiredId();
            await _settingService.UpdateMailSetting(viewModel, userId);

            return NoContent();
        }
        catch (UserNotSignedInException ex)
        {
            _logger.LogWarning(ex, "User not signed in when update mail settings.");
            return Conflict(L["A felhasználó nincs bejelentkezve"].Value);
        }
        catch (MissingSettingException ex)
        {
            _logger.LogWarning(ex, "Mail settings not found.");
            return Conflict(L["Mail beállítások nem találhatóak"].Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during update mail settings.");
            return BadRequest(L["Mail beállítások mentése nem sikerült"].Value);
        }
    }

    [HttpPatch, Route("update-email-template", Name = ApiRoutes.UpdateEmailTemplate)]
    public async Task<IActionResult> UpdateEmailTemplate(EmailTemplateType emailTemplateType, EditEmailTemplateViewModel viewModel)
    {
        try
        {
            await _settingService.UpdateEmailTemplate(emailTemplateType, viewModel);

            return NoContent();
        }
        catch (MissingSettingException ex)
        {
            if (ex.AppSettingName == nameof(EmailTemplate))
            {
                _logger.LogWarning(ex, "Email template not found.");
                return Conflict(L["Email sablon nem található"].Value);
            }

            _logger.LogWarning(ex, "Mail settings not found.");
            return Conflict(L["Mail beállítások nem találhatóak"].Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during update email template.");
            return BadRequest(L["Email sablon mentése nem sikerült"].Value);
        }
    }

    [HttpPost, Route("create-email-template-value", Name = ApiRoutes.CreateEmailTemplateValue)]
    public async Task<IActionResult> CreateEmailTemplateValue(CreateEmailTemplateValueViewModel viewModel)
    {
        try
        {
            await _settingService.CreateEmailTemplateValue(viewModel);

            return NoContent();
        }
        catch (MissingSettingException ex)
        {
            _logger.LogWarning(ex, "Mail settings not found.");
            return Conflict(L["Mail beállítások nem találhatóak"].Value);
        }
        catch (EmailTemplateValueExistsException ex)
        {
            _logger.Warning(ex, $"Email template value already exists with same key: {ex.Key}");
            return Conflict(L["Email sablon változó érték már létezik ezzel a kulcsal: {0}", ex.Key].Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during create email template value.");
            return BadRequest(L["Email sablon változó érték létrehozása nem sikerült"].Value);
        }
    }

    [HttpPut, Route("update-email-template-value", Name = ApiRoutes.UpdateEmailTemplateValue)]
    public async Task<IActionResult> UpdateEmailTemplateValue(string key, [FromBody] EditEmailTemplateValueViewModel viewModel)
    {
        try
        {
            await _settingService.UpdateEmailTemplateValue(key, viewModel);

            return NoContent();
        }
        catch (MissingSettingException ex)
        {
            if (ex.AppSettingName == nameof(EmailTemplateValue))
            {
                _logger.LogWarning(ex, "Email template value not found.");
                return Conflict(L["Email változó érték nem található"].Value);
            }

            _logger.LogWarning(ex, "Mail settings not found.");
            return Conflict(L["Mail beállítások nem találhatóak"].Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during update email template value.");
            return BadRequest(L["Email sablon változó érték mentése nem sikerült"].Value);
        }
    }

    [HttpDelete, Route("delete-email-template-value", Name = ApiRoutes.DeleteEmailTemplateValue)]
    public async Task<IActionResult> DeleteEmailTemplateValue(string key)
    {
        try
        {
            await _settingService.DeleteEmailTemplateValue(key);

            return NoContent();
        }
        catch (MissingSettingException ex)
        {
            if (ex.AppSettingName == nameof(EmailTemplateValue))
            {
                _logger.LogWarning(ex, "Email template value not found.");
                return Conflict(L["Email változó érték nem található"].Value);
            }

            _logger.LogWarning(ex, "Mail settings not found.");
            return Conflict(L["Mail beállítások nem találhatóak"].Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during delete email template value.");
            return BadRequest(L["Email sablon változó érték törlése nem sikerült"].Value);
        }
    }
}
