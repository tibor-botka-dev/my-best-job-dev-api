using AutoMapper;
using MyBestJob.BLL.Exceptions;
using MyBestJob.BLL.Stuff;
using MyBestJob.BLL.ViewModels;
using MyBestJob.BLL.ViewModels.Authentication;
using MyBestJob.DAL.Database;
using MyBestJob.DAL.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoFramework.Linq;
using Newtonsoft.Json;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using static MyBestJob.DAL.Constants.Constants;

namespace MyBestJob.BLL.Services;

public interface IInitService
{
    Task InsertDefaultData();
    Task<GetIdleSettingViewModel> GetIdleSetting(Guid? userId = null);
    Task<List<RoleViewModel>> GetAndAddPolicyRoles(ClaimsPrincipal user);
    (string signInUrl, string signUpUrl) GetCheckGoogleUrls();
}

public class InitService(
    ILogger<InitService> logger,
    IMapper mapper,
    UserManager<User> userManager,
    MyBestJobDbContext context,
    IOptions<AuthorizationOptions> authorizationOptions,
    IAuthorizationService authorizationService,
    IOptions<MailSetting> mailSetting,
    IOptions<AdminSetting> adminSetting,
    IOptions<IdleSetting> idleSetting,
    IOptions<GoogleSetting> googleSetting) : IInitService
{
    private readonly ILogger<InitService> _logger = logger;
    private readonly IMapper _mapper = mapper;

    private readonly UserManager<User> _userManager = userManager;
    private readonly MyBestJobDbContext _context = context;
    private readonly AuthorizationOptions _authorizationOptions = authorizationOptions.Value;
    private readonly IAuthorizationService _authorizationService = authorizationService;

    private readonly MailSetting _mailSetting = mailSetting.Value;
    private readonly AdminSetting _adminSetting = adminSetting.Value;
    private readonly IdleSetting _idleSetting = idleSetting.Value;
    private readonly GoogleSetting _googleSetting = googleSetting.Value;

    public async Task<GetIdleSettingViewModel> GetIdleSetting(Guid? userId = null)
    {
        var idleSetting = await _context.IdleSettings.FirstOrDefaultAsync(x => x.CreatedByUserId == userId)
            ?? await _context.IdleSettings.FirstOrDefaultAsync(x => x.CreatedByUserId == null)
            ?? throw new MissingSettingException(nameof(IdleSetting));
        var viewModel = _mapper.Map<GetIdleSettingViewModel>(idleSetting);

        return viewModel;
    }

    public async Task InsertDefaultData()
    {
        await InsertDefaultRoles();
        await InsertDefaultAdmin();

        await InsertDefaultMailSetting();
        await InsertDefaultIdleSetting();

        await _context.SaveChangesAsync();
    }

    public async Task<List<RoleViewModel>> GetAndAddPolicyRoles(ClaimsPrincipal user)
    {
        var roles = await _context.Roles
            .AsNoTracking()
            .Where(x => !string.IsNullOrEmpty(x.Name))
            .ToListAsync();

        var result = new List<RoleViewModel>();
        foreach (var role in roles)
        {
            var policy = _authorizationOptions.GetPolicy(role.Name!);
            if (policy == null)
            {
                _authorizationOptions.AddPolicy(role.Name!, new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireRole(role.Name!)
                    .Build());

                _logger.Trace($"Role added to policy: {role}.");
            }

            var authorized = await _authorizationService.AuthorizeAsync(user, _authorizationOptions.GetPolicy(role.Name!)!);
            result.Add(new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name!.ToLower(),
                HasUserAccess = authorized.Succeeded
            });
        }

        return result;
    }

    private async Task InsertDefaultRoles()
    {
        if (await _context.Roles.AnyAsync())
            return;

        _context.Roles.AddRange(DataSeeder.Roles);
        _logger.Trace("Roles added to database.");

        await _context.SaveChangesAsync();
    }

    private async Task InsertDefaultAdmin()
    {
        if (!await _context.Users.AnyAsync(x => x.Email == _adminSetting.Email))
        {
            var adminUser = new User
            {
                Email = _adminSetting.Email,
                UserName = _adminSetting.UserName,
                FirstName = _adminSetting.FirstName,
                LastName = _adminSetting.LastName,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(adminUser, _adminSetting.Password);
            if (!createResult.Succeeded)
                throw new CanNotCreateUserException(createResult.Errors);

            var adminRole = DataSeeder.Roles.FirstOrDefault(x => x.Id == DefaultRoles.AdministratorRoleId);
            var roleResult = await _userManager.AddToRoleAsync(adminUser, adminRole?.Name ?? string.Empty);
            if (!roleResult.Succeeded)
                throw new CanNotCreateUserException(roleResult.Errors, text: $"Can not assign 'Administrator' role to Admin user - {adminUser.FullName}.");

            _logger.Trace("Admin user added to database with email address: {0}", _adminSetting.Email);
        }
    }

    private async Task InsertDefaultMailSetting()
    {
        var setting = await _context.MailSettings
            .FirstOrDefaultAsync();
        if (setting != null)
            return;

        _mailSetting.AesPassword = Crypto.EncryptSensitiveData(_mailSetting.AesPassword, _mailSetting.AesKey);
        _context.MailSettings.Add(_mailSetting);
        _logger.Trace("Mail settings added to database with email address: {0}", _mailSetting.Email);
    }

    private async Task InsertDefaultIdleSetting()
    {
        var setting = await _context.IdleSettings
            .FirstOrDefaultAsync(x => x.CreatedByUserId == null);
        if (setting != null)
            return;

        _context.IdleSettings.Add(_idleSetting);
        _logger.Trace("Idle settings added to database.");
    }

    public (string signInUrl, string signUpUrl) GetCheckGoogleUrls()
    {
        var googleSignInUrlViewModel = _mapper.Map<GoogleUrlViewModel>(_googleSetting);
        googleSignInUrlViewModel.RedirectUri = _googleSetting.SignInRedirectUrl;

        var googleSignUpUrlViewModel = _mapper.Map<GoogleUrlViewModel>(_googleSetting);
        googleSignUpUrlViewModel.RedirectUri = _googleSetting.SignUpRedirectUrl;

        var googleSignInUrlProperties = googleSignInUrlViewModel.GetType().GetProperties()
            .Where(x => x.GetValue(googleSignInUrlViewModel, null) != null)
            .Select(x => $"{x.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? x.Name}={HttpUtility.UrlEncode(x.GetValue(googleSignInUrlViewModel, null)?.ToString())}");

        var googleSignUpUrlProperties = googleSignUpUrlViewModel.GetType().GetProperties()
            .Where(x => x.GetValue(googleSignUpUrlViewModel, null) != null)
            .Select(x => $"{x.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? x.Name}={HttpUtility.UrlEncode(x.GetValue(googleSignUpUrlViewModel, null)?.ToString())}");

        var googleSignInUrl = $"{_googleSetting.AuthUrl}?{string.Join("&", googleSignInUrlProperties)}";
        var googleSignUpUrl = $"{_googleSetting.AuthUrl}?{string.Join("&", googleSignUpUrlProperties)}";

        return (googleSignInUrl, googleSignUpUrl);
    }

    //public FacebookSetting GetFacebookSetting()
    //    => _facebookSetting;
}
