using System.ComponentModel.DataAnnotations;

namespace MyBestJob.DAL.Database.Models;

public class GoogleSetting
{
    [Required]
    public string ClientId { get; set; } = string.Empty;
    [Required]
    public string ClientSecret { get; set; } = string.Empty;
    [Required]
    public string ProjectId { get; set; } = string.Empty;
    [Required]
    public string AuthUrl { get; set; } = string.Empty;
    [Required]
    public string SignInRedirectUrl { get; set; } = string.Empty;
    [Required]
    public string SignUpRedirectUrl { get; set; } = string.Empty;
    [Required]
    public string TokenUrl { get; set; } = string.Empty;
    [Required]
    public string GetUserDataUrl { get; set; } = string.Empty;
    [Required]
    public string Scope { get; set; } = string.Empty;
}