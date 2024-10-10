using MyBestJob.BLL.Attributes;
using MyBestJob.BLL.Exceptions;
using MyBestJob.BLL.Services;
using MyBestJob.BLL.Stuff;
using MyBestJob.BLL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using static MyBestJob.DAL.Constants.Constants;

namespace MyBestJob.API.Controllers;

[ApiController, Route("api/auth")]
[AuthorizeByRole]
public class AuthenticationController(ILogger<AuthenticationController> logger,
    IStringLocalizer<AuthenticationController> localizer,
    ITokenService tokenService,
    IAuthenticationService authenticationService,
    IUserService userService) : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger = logger;
    private readonly IStringLocalizer L = localizer;

    private readonly ITokenService _tokenService = tokenService;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IUserService _userService = userService;

    [AllowAnonymous]
    [HttpPost, Route("sign-in", Name = ApiRoutes.SignIn)]
    public async Task<IActionResult> SignIn(SignInViewModel viewModel)
    {
        try
        {
            var response = await _authenticationService.SignIn(viewModel);

            return Ok(response);
        }
        catch (UserNotFoundException ex)
        {
            _logger.Warning(ex, $"User not found with email: {ex.Email}.");
            return Conflict(L["A felhasználó nem található a következő email címmel", ex.Email!].Value);
        }
        catch (IncorrectPasswordException ex)
        {
            _logger.LogWarning(ex, "Wrong password.");
            return Conflict(L["Helytelen jelszó"].Value);
        }
        catch (EmailNotConfirmedException ex)
        {
            _logger.Warning(ex, $"Email address is not verified: {ex.Email}.");
            return Conflict(L["Az email cím nincs hitelesítve: {0}", ex.Email].Value);
        }
        catch (UserNotSignedInException ex)
        {
            _logger.LogWarning(ex, "User not logged in. Id claim is missing.");
            return Conflict(L["A bejelentkezés nem sikerült"].Value);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during sign in.");
            return BadRequest(L["A bejelentkezés nem sikerült"].Value);
        }
    }

    [AllowAnonymous]
    [HttpPost, Route("sign-up", Name = ApiRoutes.SignUp)]
    public async Task<IActionResult> SignUp(SignUpViewModel viewModel)
    {
        try
        {
            await _authenticationService.SignUp(viewModel);

            return Ok();
        }
        catch (UserExistsException ex)
        {
            return Conflict(L["A felhasználó már regisztrált: '{0}'", ex.Email].Value);
        }
        catch (CanNotCreateUserException ex)
        {
            var errorMessage = ex.Errors!.GetErrorMessage() ?? string.Empty;

            _logger.Error(ex, $"Can not create user: {errorMessage}");
            return Conflict(L["Hiba történt a felhasználó létrehozása közben"].Value);
        }
        catch (CanNotSendEmailException ex)
        {
            _logger.Error(ex, $"Can not send sign up email: {ex.Message}");
            return Conflict(L["A regisztrációt hitelesítő email kiküldése nem sikerült: '{0}'", ex.Email].Value);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error during sign up: {ex.Message}");
            return BadRequest(L["A regisztráció nem sikerült"].Value);
        }
    }

    [AllowAnonymous]
    [HttpPost, Route("forgot-password", Name = ApiRoutes.ForgotPassword)]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
    {
        try
        {
            await _authenticationService.ForgotPassword(viewModel.Email);

            return Ok();
        }
        catch (UserNotFoundException ex)
        {
            _logger.Warning(ex, $"User not found with email: {ex.Email}.");
            return Conflict(L["A felhasználó nem található a következő email címmel", ex.Email!].Value);
        }
        catch (EmailNotConfirmedException ex)
        {
            _logger.Warning(ex, $"Email address is not verified: {ex.Email}.");
            return Conflict(L["Az email cím nincs hitelesítve", ex.Email].Value);
        }
        catch (CanNotChangePasswordOfExternalAccountException ex)
        {
            _logger.Warning(ex, $"Can not change password with email: '{ex.Email}'. Account is external.");
            return Conflict(L["Új jelszó nem igényelhető, mert '{0}' email című fiókja Google vagy Facebook fiókhoz tartozik", ex.Email]);
        }
        catch (CanNotSendEmailException ex)
        {
            _logger.Warning(ex, $"The email requesting a new password could not be sent: {ex.Email}.");
            return Conflict(L["Új jelszó igénylő email kiküldése nem sikerült a(z) '{0}' email címre", ex.Email].Value);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during forgot password.");
            return BadRequest(L["Új jelszó igénylése nem sikerült", ex.Message].Value);
        }
    }

    [AllowAnonymous]
    [HttpPost, Route("reset-password", Name = ApiRoutes.ResetPassword)]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
    {
        try
        {
            await _authenticationService.ResetPassword(viewModel);

            return Ok();
        }
        catch (UserNotFoundException ex)
        {
            _logger.Warning(ex, $"User not found with email: {ex.Email}.");
            return Conflict(L["A felhasználó nem található a következő email címmel", ex.Email!].Value);
        }
        catch (EmailNotConfirmedException ex)
        {
            _logger.Warning(ex, $"Email address is not verified: {ex.Email}.");
            return Conflict(L["Az email cím nincs hitelesítve", ex.Email].Value);
        }
        catch (CanNotChangePasswordOfExternalAccountException ex)
        {
            _logger.Warning(ex, $"Can not reset password with email: '{ex.Email}'. Account is external.");
            return Conflict(L["Új jelszó beállítása nem sikerült, mert '{0}' email című fiókja Google vagy Facebook fiókhoz tartozik", ex.Email]);
        }
        catch (CanNotResetPasswordException ex)
        {
            var errorMessage = ex.Errors.GetErrorMessage();
            _logger.Warning(ex, $"Setting a new password failed with the following email address: '{ex.Email}' and errors: {errorMessage}.");
            return Conflict(L["Új jelszó beállítása nem sikerült a(z) '{0}' email címmel", ex.Email].Value);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during reset password.");
            return BadRequest(L["Új jelszó beállítása nem sikerült"].Value);
        }
    }

    [AllowAnonymous]
    [HttpGet, Route("refresh-token", Name = ApiRoutes.RefreshToken)]
    public async Task<IActionResult> RefreshToken(string accessToken, string refreshToken)
    {
        try
        {
            var newToken = await _tokenService.RefreshToken(accessToken, refreshToken);

            return Ok(newToken);
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning(ex, "Invalid token.");
            return Conflict(L["Érvénytelen token"].Value);
        }
        catch (UserNotSignedInException ex)
        {
            _logger.LogWarning(ex, "User not logged in. Id claim is missing.");
            return Conflict(L["Token generálása nem sikerült. A felhasználó nincs belépve"].Value);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during refresh token.");
            return BadRequest(L["Token generálása nem sikerült"].Value);
        }
    }

    [AllowAnonymous]
    [HttpPost, Route("confirm-account", Name = ApiRoutes.ConfirmAccount)]
    public async Task<IActionResult> ConfirmAccount(ConfirmAccountViewModel viewModel)
    {
        try
        {
            await _authenticationService.ConfirmAccount(viewModel);

            return Ok();
        }
        catch (UserNotFoundException ex)
        {
            return Conflict(L["A felhasználó nem található a következő email címmel vagy felhasználónévvel", ex.Email!].Value);
        }
        catch (EmailAlreadyConfirmedException ex)
        {
            return Conflict(L["Az email cím már hitelesítve van", ex.Email].Value);
        }
        catch (CanNotConfirmEmailException ex)
        {
            var errorMessage = ex.Errors!.GetErrorMessage();
            _logger.Warning(ex, $"Error during confirm account: '{errorMessage}'.");
            return Conflict(L["A felhasználót '{0}' töröltük, mert hiba történt az email cím hitelesítése közben. Kérjük regisztráljon újra.", viewModel.Email].Value);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during confirm account.");
            return BadRequest(L["Az email cím hitelesítése nem sikerült", ex.Message].Value);
        }
    }

    [HttpPost, Route("change-password", Name = ApiRoutes.ChangePassword)]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
    {
        try
        {
            var user = await _userService.GetRequiredCurrentUser(User.Claims);

            await _authenticationService.ChangePassword(viewModel, user);
            return Ok();
        }
        catch (CanNotChangePasswordException ex)
        {
            var errorMessage = ex.Errors.GetErrorMessage();
            _logger.Warning(ex, $"Setting a new password failed with the following email address: '{ex.Email}' and errors: {errorMessage}.");
            return Conflict(L["Új jelszó beállítása nem sikerült a(z) '{0}' email címmel", ex.Email].Value);
        }
        catch (UserNotSignedInException ex)
        {
            _logger.Warning(ex, "User not signed in.");
            return Conflict(L["A felhasználó nincs bejelentkezve"].Value);
        }
        catch (EmailNotConfirmedException ex)
        {
            _logger.Warning(ex, $"Email address is not verified: {ex.Email}.");
            return Conflict(L["Az email cím nincs hitelesítve", ex.Email].Value);
        }
        catch (CanNotChangePasswordOfExternalAccountException ex)
        {
            _logger.Warning(ex, $"Can not reset password with email: '{ex.Email}'. Account is external.");
            return Conflict(L["Új jelszó beállítása nem sikerült, mert '{0}' email című fiókja Google vagy Facebook fiókhoz tartozik", ex.Email]);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during change password.");
            return BadRequest(L["Új jelszó beállítása nem sikerült"].Value);
        }
    }

    [HttpPost, Route("sign-out", Name = ApiRoutes.SignOut)]
    public async Task<IActionResult> SignOut(SignOutViewModel viewModel)
    {
        try
        {
            await _authenticationService.SignOut(viewModel.AccessToken);

            return Ok();
        }
        catch (UserNotFoundException ex)
        {
            _logger.Warning(ex, $"User not found.");
            return Conflict(L["A felhasználó nem található"].Value);
        }
        catch (SecurityTokenException ex)
        {
            _logger.Warning(ex, "Invalid token.");
            return Conflict(L["Érvénytelen token"].Value);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during sign out.");
            return BadRequest(L["A kijelentkezés nem sikerült"].Value);
        }
    }
}
