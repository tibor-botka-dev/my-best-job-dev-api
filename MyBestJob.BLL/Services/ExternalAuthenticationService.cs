using AutoMapper;
using MyBestJob.BLL.Exceptions;
using MyBestJob.BLL.Stuff;
using MyBestJob.BLL.ViewModels;
using MyBestJob.DAL.Constants;
using MyBestJob.DAL.Database.Models;
using MyBestJob.DAL.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MyBestJob.BLL.Services;

public interface IExternalAuthenticationService
{
    Task SignUp(ExternalSignUpViewModel viewModel);
    Task<SignInResponseViewModel> SignIn(string email);
    Task<GoogleUserDataResponseViewModel> GetGoogleUserData(string code, bool isSignIn = true);
}

public class ExternalAuthenticationService(ILogger<ExternalAuthenticationService> logger,
    IStringLocalizer<AuthenticationService> localizer,
    UserManager<User> userManager,
    IMapper mapper,
    IHttpService httpService,
    IRoleService roleService,
    IEmailService emailService,
    ITokenService tokenService,
    IUserService userService,
    IOptions<GoogleSetting> googleSetting) : IExternalAuthenticationService
{
    private readonly ILogger<ExternalAuthenticationService> _logger = logger;
    private readonly IStringLocalizer L = localizer;

    private readonly UserManager<User> _userManager = userManager;
    private readonly IMapper _mapper = mapper;
    private readonly IHttpService _httpService = httpService;
    private readonly IRoleService _roleService = roleService;
    private readonly IEmailService _emailService = emailService;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IUserService _userService = userService;
    private readonly GoogleSetting _googleSetting = googleSetting.Value;

    public async Task SignUp(ExternalSignUpViewModel viewModel)
    {
        var user = await _userManager.FindByEmailAsync(viewModel.Email)
            ?? await _userManager.FindByNameAsync(viewModel.Email);
        if (user != null)
            throw new UserExistsException(viewModel.Email);

        user = _mapper.Map<User>(viewModel);

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
            throw new CanNotCreateUserException(result.Errors);

        try
        {
            await _emailService.SendEmail(EmailTemplateType.ConfirmAccount, user);

            _logger.Info($"User {user.FullName} - {user.Email} signed up successfully with Google.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Can not send confirm account email to {viewModel.Email}.");
            await _userManager.DeleteAsync(user);
            throw new CanNotSendEmailException(user.Email!, "Can not send confirm account email.");
        }
    }

    public async Task<SignInResponseViewModel> SignIn(string email)
    {
        var user = await _userManager.FindByEmailAsync(email)
                ?? await _userManager.FindByNameAsync(email)
                ?? throw new UserNotFoundException(email);

        if (!await _userManager.IsEmailConfirmedAsync(user))
            throw new EmailNotConfirmedException(email);

        _logger.Info($"User: {user.FullName} - {user.Email} logged in with Google.");

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

    public async Task<GoogleUserDataResponseViewModel> GetGoogleUserData(string code, bool isSignIn = true)
    {
        _httpService.BaseUrl = _googleSetting.TokenUrl;
        var tokenRequest = new GoogleTokenRequestViewModel
        {
            Code = code,
            ClientId = _googleSetting.ClientId,
            ClientSecret = _googleSetting.ClientSecret,
            RedirectUri = isSignIn ? _googleSetting.SignInRedirectUrl : _googleSetting.SignUpRedirectUrl,
            GrantType = Constants.Google.GrantType
        };

        var tokenResponse = await _httpService.Post<GoogleTokenResponseViewModel>("token",
            tokenRequest,
            HttpRequestType.FormUrlEncodedContent);

        if (!tokenResponse.IsSuccessStatusCode || tokenResponse.Object is null)
            throw new GoogleTokenException("Can not get Google token.");

        _httpService.BaseUrl = _googleSetting.GetUserDataUrl;
        _httpService.Token = tokenResponse.Object.IdToken;

        var userDataResponse = await _httpService.Get<GoogleUserDataResponseViewModel>(
            $"userinfo?alt=json&access_token={tokenResponse.Object.AccessToken}");

        return !userDataResponse.IsSuccessStatusCode || userDataResponse.Object is null
            ? throw new GoogleUserDataException("Can not get Google user data.")
            : userDataResponse.Object;
    }
}
