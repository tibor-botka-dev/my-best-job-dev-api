using MyBestJob.BLL.Exceptions;
using MyBestJob.BLL.Services;
using MyBestJob.BLL.Stuff;
using MyBestJob.BLL.ViewModels;
using MyBestJob.DAL.Database.Models;
using MyBestJob.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static MyBestJob.DAL.Constants.Constants;

namespace MyBestJob.API.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/external-auth")]
public class ExternalAuthenticationController(ILogger<ExternalAuthenticationController> logger,
    IStringLocalizer<ExternalAuthenticationController> localizer,
    IExternalAuthenticationService externalAuthenticationService,
    IOptions<RouteSetting> routeSetting) : ControllerBase
{
    private readonly ILogger<ExternalAuthenticationController> _logger = logger;
    private readonly IStringLocalizer L = localizer;

    private readonly IExternalAuthenticationService _externalAuthenticationService = externalAuthenticationService;
    private readonly RouteSetting _routeSetting = routeSetting.Value;

    [HttpGet, Route("google-sign-in", Name = ApiRoutes.GoogleSignIn)]
    public async Task<IActionResult> GoogleSignIn([FromQuery] GoogleRequestViewModel request)
    {
        var signInUrl = _routeSetting.Routes.GetFrontEndCallbackUrl(RouteType.SignIn);

        try
        {
            var googleUserData = await _externalAuthenticationService.GetGoogleUserData(request.Code);
            var response = await _externalAuthenticationService.SignIn(googleUserData.Email);

            var tokens = JsonConvert.SerializeObject(response.Tokens, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            signInUrl += $"?isSuccess=true&tokens={tokens}&avatar={response.Avatar}";
        }
        catch (GoogleTokenException ex)
        {
            _logger.Error(ex, $"Can not get Google token: {ex.Message}");
            signInUrl += $"?error={L["Google autentikáció nem sikerült"].Value}";
        }
        catch (GoogleUserDataException ex)
        {
            _logger.Error(ex, $"Can not get Google user data: {ex.Message}");
            signInUrl += $"?error={L["Google felhasználó adatok lekérdezése nem sikerült"].Value}";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error during Google sign in: {ex.Message}");
            signInUrl += $"?error={L["A Google bejelentkezés nem sikerült"].Value}";
        }

        return Redirect(signInUrl);
    }

    [HttpGet, Route("google-sign-up", Name = ApiRoutes.GoogleSignUp)]
    public async Task<IActionResult> GoogleSignUp([FromQuery] GoogleRequestViewModel request)
    {
        var signUpUrl = _routeSetting.Routes.GetFrontEndCallbackUrl(RouteType.SignUp);

        try
        {
            var googleUserData = await _externalAuthenticationService.GetGoogleUserData(request.Code, false);
            var viewModel = new ExternalSignUpViewModel
            {
                Email = googleUserData.Email,
                FirstName = googleUserData.GivenName,
                LastName = googleUserData.FamilyName,
                AvatarUrl = googleUserData.Picture,
                IsGoogleAccount = true
            };

            await _externalAuthenticationService.SignUp(viewModel);

            signUpUrl += $"?isSuccess=true";
        }
        catch (HttpErrorException ex)
        {
            _logger.Error(ex, $"Error during HttpClient call: {ex.Message}");
            signUpUrl += $"?error={L["Nem sikerült elérni a Google szolgáltatást"].Value}";
        }
        catch (GoogleTokenException ex)
        {
            _logger.Error(ex, $"Can not get Google token: {ex.Message}");
            signUpUrl += $"?error={L["Google autentikáció nem sikerült"].Value}";
        }
        catch (GoogleUserDataException ex)
        {
            _logger.Error(ex, $"Can not get Google user data: {ex.Message}");
            signUpUrl += $"?error={L["Google felhasználó adatok lekérdezése nem sikerült"].Value}";
        }
        catch (UserExistsException ex)
        {
            _logger.Error(ex, $"User - '{ex.Email}' already exists: {ex.Message}");
            signUpUrl += $"?error={L["A felhasználó már regisztrált: '{0}'", ex.Email].Value}";
        }
        catch (CanNotCreateUserException ex)
        {
            var errorMessage = ex.Errors!.GetErrorMessage() ?? string.Empty;

            _logger.Error(ex, $"Can not create user: {errorMessage}");
            signUpUrl += $"?error={L["Hiba történt a felhasználó létrehozása közben"].Value}";
        }
        catch (CanNotSendEmailException ex)
        {
            _logger.Error(ex, $"Can not send Google sign up email: {ex.Message}");
            signUpUrl += $"?error={L["A regisztrációt hitelesítő email kiküldése nem sikerült: '{0}'", ex.Email].Value}";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error during Google sign up: {ex.Message}");
            signUpUrl += $"?error={L["A Google regisztráció nem sikerült"].Value}";
        }

        return Redirect(signUpUrl);
    }
}
