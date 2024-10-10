using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using MyBestJob.BLL.Exceptions;
using MyBestJob.BLL.Exceptions.Service;
using MyBestJob.BLL.Stuff;
using MyBestJob.BLL.ViewModels;
using MyBestJob.DAL.Database;
using MyBestJob.DAL.Database.Models;
using MyBestJob.DAL.Enums;

namespace MyBestJob.BLL.Services;

public interface IAuthenticationService
{
    Task<SignInResponseViewModel> SignIn(SignInViewModel viewModel);
    Task SignOut(string accessToken);
    Task ConfirmAccount(ConfirmAccountViewModel viewModel);
    Task ForgotPassword(string email);
    Task ResetPassword(ResetPasswordViewModel viewModel);
    Task ChangePassword(ChangePasswordViewModel viewModel, User user);
    Task SignUp(SignUpViewModel viewModel);
}

public class AuthenticationService(ILogger<AuthenticationService> logger,
    MyBestJobDbContext context,
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IRoleService roleService,
    IEmailService emailService,
    ITokenService tokenService,
    IUserService userService,
    IMapper mapper) : IAuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger = logger;

    private readonly MyBestJobDbContext _context = context;
    private readonly UserManager<User> _userManager = userManager;
    private readonly SignInManager<User> _signInManager = signInManager;
    private readonly IRoleService _roleService = roleService;
    private readonly IEmailService _emailService = emailService;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IUserService _userService = userService;
    private readonly IMapper _mapper = mapper;

    public async Task ChangePassword(ChangePasswordViewModel viewModel, User user)
    {
        if (!await _userManager.IsEmailConfirmedAsync(user))
            throw new EmailNotConfirmedException(user.Email!);

        if (user.IsGoogleAccount || user.IsFacebookAccount)
            throw new CanNotChangePasswordOfExternalAccountException(user.Email!);

        var result = await _userManager.ChangePasswordAsync(user, viewModel.OldPassword, viewModel.Password);
        if (!result.Succeeded)
            throw new CanNotChangePasswordException(result.Errors, user.Email!);
    }

    public async Task ConfirmAccount(ConfirmAccountViewModel viewModel)
    {
        var user = await _userManager.FindByEmailAsync(viewModel.Email)
            ?? await _userManager.FindByNameAsync(viewModel.Email)
            ?? throw new UserNotFoundException(viewModel.Email);

        if (await _userManager.IsEmailConfirmedAsync(user))
            throw new EmailAlreadyConfirmedException(viewModel.Email, "Email is already confirmed.");

        var result = await _userManager.ConfirmEmailAsync(user, viewModel.Token);
        if (!result.Succeeded)
        {
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
                throw new CanNotDeleteUserException(deleteResult.Errors);

            throw new CanNotConfirmEmailException(result.Errors);
        }
    }

    public async Task ForgotPassword(string email)
    {
        var user = await _userManager.FindByEmailAsync(email)
            ?? await _userManager.FindByNameAsync(email)
            ?? throw new UserNotFoundException(email);

        if (!await _userManager.IsEmailConfirmedAsync(user))
            throw new EmailNotConfirmedException(email);

        if (user.IsGoogleAccount || user.IsFacebookAccount)
            throw new CanNotChangePasswordOfExternalAccountException(email);

        try
        {
            await _emailService.SendEmail(EmailTemplateType.ResetPassword, user);
            _logger.Info($"Forgot password email sent to user - {user.Id} successfully.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Can not send forgot password email to user - {user.Id}.");
            throw new CanNotSendEmailException(email, "Can not send forgot password email.");
        }
    }

    public async Task ResetPassword(ResetPasswordViewModel viewModel)
    {
        var user = await _userManager.FindByEmailAsync(viewModel.Email)
            ?? await _userManager.FindByNameAsync(viewModel.Email)
            ?? throw new UserNotFoundException(viewModel.Email);

        if (!await _userManager.IsEmailConfirmedAsync(user))
            throw new EmailNotConfirmedException(viewModel.Email);

        if (user.IsGoogleAccount || user.IsFacebookAccount)
            throw new CanNotChangePasswordOfExternalAccountException(viewModel.Email);

        var result = await _userManager.ResetPasswordAsync(user, viewModel.Token, viewModel.Password);
        if (!result.Succeeded)
            throw new CanNotResetPasswordException(result.Errors, viewModel.Email);
    }

    public async Task<SignInResponseViewModel> SignIn(SignInViewModel viewModel)
    {
        var user = await _userManager.FindByEmailAsync(viewModel.Email)
                ?? await _userManager.FindByNameAsync(viewModel.Email)
                ?? throw new UserNotFoundException(viewModel.Email);

        if (!string.IsNullOrEmpty(viewModel.Password))
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, viewModel.Password!, false);
            if (!result.Succeeded)
                throw new IncorrectPasswordException("Incorrect password.");
        }

        if (!await _userManager.IsEmailConfirmedAsync(user))
            throw new EmailNotConfirmedException(viewModel.Email);

        _logger.Info($"User - {user.Id} logged in.");

        var claims = await _roleService.GetUserClaims(user);
        var tokens = await _tokenService.GenerateTokens(claims);

        var userId = await claims.GetRequiredId();
        var avatar = await _userService.GetUserAvatar(userId);

        var response = new SignInResponseViewModel
        {
            Tokens = tokens,
            Avatar = avatar
        };

        return response;
    }

    public async Task SignUp(SignUpViewModel viewModel)
    {
        var user = await _userManager.FindByEmailAsync(viewModel.Email)
            ?? await _userManager.FindByNameAsync(viewModel.Email);
        if (user != null)
            throw new UserExistsException(viewModel.Email);

        user = _mapper.Map<User>(viewModel);

        var result = await _userManager.CreateAsync(user, viewModel.Password);
        if (!result.Succeeded)
            throw new CanNotCreateUserException(result.Errors);

        try
        {
            await _emailService.SendEmail(EmailTemplateType.ConfirmAccount, user);

            _logger.Info($"User - {user.Id} signed up successfully.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Can not send confirm account email to {viewModel.Email}.");
            await _userManager.DeleteAsync(user);
            throw new CanNotSendEmailException(user.Email!, "Can not send confirm account email.");
        }
    }

    public async Task SignOut(string accessToken)
    {
        var claims = await _tokenService.GetClaimsFromExpiredToken(accessToken);

        var userId = await claims.GetRequiredId();
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId)
            ?? throw new UserNotFoundException();

        user.Tokens.Clear();

        await _context.SaveChangesAsync();

        _logger.Info("User logged out.");
    }
}
