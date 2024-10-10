using System.ComponentModel.DataAnnotations;

namespace MyBestJob.DAL.Database.Models;

public class JwtSetting
{
    [Required]
    public bool ValidateIssuerSigningKey { get; set; }
    [Required]
    public string IssuerSigningKey { get; set; } = string.Empty;

    [Required]
    public bool ValidateIssuer { get; set; }
    [Required]
    public string ValidIssuer { get; set; } = string.Empty;

    [Required]
    public bool ValidateAudience { get; set; }
    [Required]
    public string ValidAudience { get; set; } = string.Empty;

    [Required]
    public bool RequireExpirationTime { get; set; }

    [Required]
    public bool ValidateLifetime { get; set; }

    [Required]
    public TimeSpan AccessTokenExpiration { get; set; }

    [Required]
    public TimeSpan RefreshTokenExpiration { get; set; }
}
