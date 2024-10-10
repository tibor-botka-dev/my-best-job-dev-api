using MyBestJob.BLL.Attributes;
using System.ComponentModel.DataAnnotations;
using static MyBestJob.DAL.Constants.Constants;

namespace MyBestJob.BLL.ViewModels;

public class SignInViewModel
{
    [Required(ErrorMessage = "Email cím mező kötelező")]
    [EmailAddress(ErrorMessage = "Email cím mező nem érvényes email cím")]
    [RegularExpression(RegexPatterns.Email)]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [RequiredIf("IsGoogleAccount", false, ErrorMessage = "Jelszó mező kötelező")]
    [StringLength(20, ErrorMessage = "Jelszó mező 6-20 karakter hosszú lehet", MinimumLength = 6)]
    [RegularExpression(RegexPatterns.Password)]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}
