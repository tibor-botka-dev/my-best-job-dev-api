using System.ComponentModel.DataAnnotations;
using static MyBestJob.DAL.Constants.Constants;

namespace MyBestJob.BLL.ViewModels;

public class EditMailSettingViewModel
{
    [Required(ErrorMessage = "Email cím mező kötelező")]
    [EmailAddress(ErrorMessage = "Email cím mező nem érvényes email cím")]
    [RegularExpression(RegexPatterns.Email)]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Név mező kötelező")]
    [StringLength(30, ErrorMessage = "Név mező 1-30 karakter hosszú lehet", MinimumLength = 1)]
    [RegularExpression(RegexPatterns.Name)]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Jelszó mező kötelező")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Host mező kötelező")]
    [RegularExpression(RegexPatterns.SmtpHost)]
    public string Host { get; set; } = string.Empty;

    [Required(ErrorMessage = "Port mező kötelező")]
    [RegularExpression(RegexPatterns.SmtpPort)]
    public int Port { get; set; }
}
