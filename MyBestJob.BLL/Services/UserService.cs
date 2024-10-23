using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using MyBestJob.BLL.Exceptions;
using MyBestJob.BLL.Exceptions.Service;
using MyBestJob.BLL.Stuff;
using MyBestJob.BLL.ViewModels;
using MyBestJob.DAL.Constants;
using MyBestJob.DAL.Database;
using MyBestJob.DAL.Database.Models;
using MyBestJob.DAL.Enums;
using System.Security.Claims;

namespace MyBestJob.BLL.Services;

public interface IUserService
{
    Task<User?> GetCurrentUser(IEnumerable<Claim> claims);
    Task<User> GetRequiredCurrentUser(IEnumerable<Claim> claims);
    Task<User> GetRequiredUser(Guid id);
    Task<GetUserViewModel> GetRequiredUserViewModel(Guid id);
    Task<string?> GetUserAvatar(Guid userId);

    Task SaveTokens(IEnumerable<Claim> claims, JwtTokenViewModel jwtTokenViewModel);
    Task UpdateUser(EditUserViewModel viewModel);
    Task CreateUser(CreateUserViewModel viewModel);
    Task DeleteUser(Guid id);

    Task<List<GetUserViewModel>> GetUsers();
}

public class UserService(ILogger<UserService> logger,
    MyBestJobDbContext context,
    UserManager<User> userManager,
    IMapper mapper,
    IEmailService emailService) : IUserService
{
    private readonly ILogger<UserService> _logger = logger;

    private readonly MyBestJobDbContext _context = context;
    private readonly UserManager<User> _userManager = userManager;
    private readonly IMapper _mapper = mapper;
    private readonly IEmailService _emailService = emailService;

    public async Task<User?> GetCurrentUser(IEnumerable<Claim> claims)
    {
        var userId = await claims.GetId();
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

        return user;
    }

    public async Task<User> GetRequiredCurrentUser(IEnumerable<Claim> claims)
    {
        var user = await GetCurrentUser(claims)
            ?? throw new UserNotSignedInException("User not logged in.");

        return user;
    }

    public async Task<User> GetRequiredUser(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new UserNotFoundException();

        return user;
    }

    public async Task<GetUserViewModel> GetRequiredUserViewModel(Guid id)
    {
        var user = await GetRequiredUser(id);
        var viewModel = _mapper.Map<GetUserViewModel>(user);

        return viewModel;
    }

    public async Task<List<GetUserViewModel>> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        var result = _mapper.Map<List<GetUserViewModel>>(users);

        return result;
    }

    public async Task<string?> GetUserAvatar(Guid userId)
    {
        var user = await GetRequiredUser(userId);

        return user.AvatarBase64 ?? user.AvatarUrl;
    }

    public async Task UpdateUser(EditUserViewModel viewModel)
    {
        var user = await GetRequiredUser(viewModel.Id);
        var isEmailChanged = !viewModel.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase);
        _mapper.Map(viewModel, user);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new CanNotUpdateUserException(result.Errors);

        if (isEmailChanged)
        {
            await _emailService.SendEmail(EmailTemplateType.ConfirmAccountWhenEmailChanged, user);
            _logger.Info($"User {user.FullName} email changed to {user.Email}.");
        }

        _logger.Info($"User {user.FullName} - {user.Email} updated successfully.");
    }

    public async Task SaveTokens(IEnumerable<Claim> claims, JwtTokenViewModel jwtTokenViewModel)
    {
        var user = await GetRequiredCurrentUser(claims);
        user.Tokens.Clear();

        user.AddUserToken(new IdentityUserToken<Guid>
        {
            UserId = user.Id,
            Name = Constants.Tokens.RefreshToken,
            Value = jwtTokenViewModel.RefreshToken
        });

        user.AddUserToken(new IdentityUserToken<Guid>
        {
            UserId = user.Id,
            Name = Constants.Tokens.RefreshTokenExpiration,
            Value = jwtTokenViewModel.RefreshTokenExpires.ToString()
        });

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            _logger.Error($"User {user.FullName} email changed to {user.Email}.");
    }

    public async Task CreateUser(CreateUserViewModel viewModel)
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
            await _emailService.SendEmail(EmailTemplateType.NewUser, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Can not send new user email.");
            throw new CanNotSendEmailException(user.Email!, "Can not send new user email.");
        }
    }

    public async Task DeleteUser(Guid id)
    {
        var user = await GetRequiredUser(id);
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new CanNotDeleteUserException(result.Errors);
    }
}
