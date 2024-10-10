using MyBestJob.BLL.Stuff;
using MyBestJob.BLL.ViewModels;
using MyBestJob.DAL.Database.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MyBestJob.BLL.Services;

public interface ITokenService
{
    Task<JwtTokenViewModel> GenerateTokens(IEnumerable<Claim> authClaims);
    Task<JwtTokenViewModel> RefreshToken(string accessToken, string refreshToken);
    Task<IEnumerable<Claim>> GetClaimsFromExpiredToken(string accessToken);
}

public class TokenService(ILogger<TokenService> logger,
    IUserService userService,
    IOptions<JwtSetting> jwtSetting) : ITokenService
{
    private readonly ILogger<TokenService> _logger = logger;

    private readonly IUserService _userService = userService;
    private readonly JwtSetting _jwtSetting = jwtSetting.Value;

    public async Task<JwtTokenViewModel> GenerateTokens(IEnumerable<Claim> claims)
    {
        var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.IssuerSigningKey));
        var accessTokenExpires = DateTime.UtcNow.Add(_jwtSetting.AccessTokenExpiration);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSetting.ValidIssuer,
            audience: _jwtSetting.ValidAudience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: accessTokenExpires,
            signingCredentials: new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256)
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        var refreshTokenExpires = DateTime.UtcNow.Add(_jwtSetting.RefreshTokenExpiration);
        var refreshToken = GenerateRefreshToken();

        var jwtToken = new JwtTokenViewModel
        {
            AccessToken = accessToken,
            AccessTokenExpires = accessTokenExpires,
            RefreshToken = refreshToken,
            RefreshTokenExpires = refreshTokenExpires
        };

        await _userService.SaveTokens(claims, jwtToken);

        _logger.Trace("Tokens generated: ", jwtToken);
        return jwtToken;
    }

    public async Task<JwtTokenViewModel> RefreshToken(string accessToken, string refreshToken)
    {
        var claims = await GetClaimsFromExpiredToken(accessToken);
        var jwtToken = await GenerateTokens(claims);

        _logger.Trace("Expired token refreshed: ", jwtToken);
        return jwtToken;
    }

    public async Task<IEnumerable<Claim>> GetClaimsFromExpiredToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = _jwtSetting.ValidateIssuerSigningKey,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.IssuerSigningKey)),
            ValidateIssuer = _jwtSetting.ValidateIssuer,
            ValidIssuer = _jwtSetting.ValidIssuer,
            ValidateAudience = _jwtSetting.ValidateAudience,
            ValidAudience = _jwtSetting.ValidAudience,
            RequireExpirationTime = _jwtSetting.RequireExpirationTime,
            ValidateLifetime = _jwtSetting.ValidateLifetime,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role
        };

        var tokenValidationResult = await new JwtSecurityTokenHandler()
            .ValidateTokenAsync(accessToken, tokenValidationParameters);

        return !tokenValidationResult.IsValid
            || tokenValidationResult.SecurityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)
            ? throw new SecurityTokenException($"Expired token is invalid: {accessToken}")
            : tokenValidationResult.ClaimsIdentity.Claims;
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);

        var refreshToken = Convert.ToBase64String(randomNumber);

        return refreshToken;
    }
}
