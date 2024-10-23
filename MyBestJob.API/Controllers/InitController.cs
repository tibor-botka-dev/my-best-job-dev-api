using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MyBestJob.API.Extensions;
using MyBestJob.BLL.Exceptions;
using MyBestJob.BLL.Services;
using MyBestJob.BLL.Stuff;
using static MyBestJob.DAL.Constants.Constants;

namespace MyBestJob.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/init", Name = ApiRoutes.Init)]
public class InitController(ILogger<InitController> logger,
    IStringLocalizer<InitController> localizer,
    IInitService initService,
    IUserService userService) : ControllerBase
{
    private readonly ILogger<InitController> _logger = logger;
    private readonly IStringLocalizer L = localizer;

    private readonly IInitService _initService = initService;
    private readonly IUserService _userService = userService;

    [HttpGet]
    public async Task<IActionResult> Init()
    {
        try
        {
            await _initService.InsertDefaultData();

            var user = await _userService.GetCurrentUser(User.Claims);
            var languages = await _initService.GetLanguages();
            var defaultLanguage = DefaultLanguages.DefaultLanguage;
            var (googleSignInUrl, googleSignUpUrl) = _initService.GetCheckGoogleUrls();

            return Ok(new
            {
                ApplicationName,
                Urls = Url.GetApiRoutes(),
                RegexPatterns = RegexPatterns.GetRegexPatterns(),
                Avatar = user?.AvatarUrl ?? user?.AvatarBase64,
                languages,
                defaultLanguage,
                googleSignInUrl,
                googleSignUpUrl
            });
        }
        catch (CanNotCreateUserException ex)
        {
            var errorMessage = ex.Errors.GetErrorMessage();
            _logger.Warning(ex, $"Error during create default users: {errorMessage}.");
            return Conflict(L["Hiba történt az alapértelmezett felhasználók létrehozása közben"].Value);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during get default data.");
            return Conflict(L["Az alapadatok betöltése nem sikerült"].Value);
        }
    }

    [HttpGet("before-load-init")]
    public async Task<IActionResult> BeforeLoadInit()
    {
        try
        {
            var roles = await _initService.GetAndAddPolicyRoles(User);

            return Ok(new
            {
                Roles = roles
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during get default data.");
            return Conflict(L["Az alapadatok betöltése nem sikerült"].Value);
        }
    }
}
