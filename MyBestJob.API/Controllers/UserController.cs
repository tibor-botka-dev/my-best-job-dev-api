using MyBestJob.BLL.Attributes;
using MyBestJob.BLL.Exceptions;
using MyBestJob.BLL.Exceptions.Service;
using MyBestJob.BLL.Services;
using MyBestJob.BLL.Stuff;
using MyBestJob.BLL.ViewModels;
using MyBestJob.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using static MyBestJob.DAL.Constants.Constants;

namespace MyBestJob.API.Controllers;

[ApiController, Route("api/user", Name = ApiRoutes.User)]
[AuthorizeByRole(nameof(Role.Administrator), nameof(Role.User))]
public class UserController(ILogger<UserController> logger,
    IStringLocalizer<UserController> localizer,
    IUserService userService) : ControllerBase
{
    private readonly ILogger<UserController> _logger = logger;
    private readonly IStringLocalizer L = localizer;

    private readonly IUserService _userService = userService;

    [HttpGet]
    [AuthorizeByRole(nameof(Role.Administrator))]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _userService.GetUsers();

            return Ok(new
            {
                users
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during get users.");
            return BadRequest(L["Felhasználók lekérdezése nem sikerült"].Value);
        }
    }

    [HttpGet, Route("get-user", Name = ApiRoutes.GetUser)]
    public async Task<IActionResult> GetUser()
    {
        try
        {
            var userId = await User.Claims.GetRequiredId();
            var profile = await _userService.GetRequiredUserViewModel(userId);

            return Ok(new
            {
                profile
            });
        }
        catch (UserNotSignedInException ex)
        {
            _logger.LogWarning(ex, "User not signed in when get profile data.");
            return Conflict(L["A felhasználó nincs bejelentkezve"].Value);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during get profile data.");
            return BadRequest(L["Személyes adatok lekérdezése nem sikerült."].Value);
        }
    }

    [HttpGet, Route("get-user-avatar", Name = ApiRoutes.GetUserAvatar)]
    public async Task<IActionResult> GetUserAvatar()
    {
        try
        {
            var userId = await User.Claims.GetRequiredId();
            var avatar = await _userService.GetUserAvatar(userId);

            return Ok(new
            {
                avatar
            });
        }
        catch (UserNotSignedInException ex)
        {
            _logger.LogWarning(ex, "User not signed in when get avatar.");
            return Conflict(L["A felhasználó nincs bejelentkezve"].Value);
        }
        catch (UserNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not signed in when get avatar.");
            return Conflict(L["A felhasználó nem található"].Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during get avatar.");
            return BadRequest(L["Személyes adatok lekérdezése nem sikerült"].Value);
        }
    }

    [HttpPost]
    [AuthorizeByRole(nameof(Role.Administrator))]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserViewModel viewModel)
    {
        try
        {
            var userId = await User.Claims.GetRequiredId();
            await _userService.CreateUser(viewModel, userId);

            return NoContent();
        }
        catch (UserNotSignedInException ex)
        {
            _logger.LogWarning(ex, "User not signed in when create user.");
            return Conflict(L["A felhasználó nincs bejelentkezve"].Value);
        }
        catch (UserExistsException ex)
        {
            _logger.Warning(ex, $"User already exists with same email: {ex.Email}");
            return Conflict(L["A felhasználó már létezik ezzel az email címmel: {0}", ex.Email].Value);
        }
        catch (CanNotCreateUserException ex)
        {
            var errorMessage = ex.Errors.GetErrorMessage();
            _logger.Warning(ex, $"Error during create user: {errorMessage}.");
            return Conflict(L["Hiba történt a felhasználó létrehozása közben"].Value);
        }
        catch (EmailTemplateValueTypeMissingException ex)
        {
            _logger.Warning(ex, $"'{ex.EmailTemplateValueType}' email template value type is missing.");
            return Conflict(L["Email kiküldése a felhasználónak nem sikerült"].Value);
        }
        catch (CanNotSendEmailException ex)
        {
            _logger.Warning(ex, $"The email requesting a new user could not be sent: {ex.Email}.");
            return Conflict(L["Email kiküldése a felhasználónak nem sikerült", ex.Email].Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during create new user.");
            return BadRequest(L["Új felhasználó létrehozása nem sikerült"].Value);
        }
    }

    [HttpPut]
    [AuthorizeByRole(nameof(Role.Administrator))]
    public async Task<IActionResult> UpdateUser([FromForm] EditUserViewModel viewModel)
    {
        try
        {
            await _userService.UpdateUser(viewModel);

            return NoContent();
        }
        catch (UserNotFoundException ex)
        {
            _logger.Warning(ex, $"User not found with email: {viewModel.Email}.");
            return Conflict(L["A felhasználó nem található a következő email címmel", viewModel.Email!].Value);
        }
        catch (InvalidFileException ex)
        {
            _logger.Warning(ex, $"Uploaded avatar is invalid. Size: {ex.Size}, type: {ex.FileType}.");
            return Conflict(L["A feltött avatar érvénytelen. Méret: {0}, típus: {1}", ex.Size, ex.FileType].Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during update user data.");
            return BadRequest(L["Személyes adatok mentése nem sikerült"].Value);
        }
    }

    [HttpDelete]
    [AuthorizeByRole(nameof(Role.Administrator))]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        try
        {
            await _userService.DeleteUser(id);

            return NoContent();
        }
        catch (CanNotDeleteUserException ex)
        {
            var errorMessage = ex.Errors.GetErrorMessage();
            _logger.Warning(ex, $"Error during delete user: {errorMessage}.");
            return Conflict(L["Hiba történt a felhasználó törlése közben"].Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during delete user.");
            return BadRequest(L["Felhasználó törlése nem sikerült"].Value);
        }
    }
}
