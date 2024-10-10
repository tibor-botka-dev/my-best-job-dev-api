using MyBestJob.BLL.Attributes;
using System.ComponentModel.DataAnnotations;
using static MyBestJob.DAL.Constants.Constants;

namespace MyBestJob.BLL.ViewModels;

public class SignUpViewModel
{
    [Required(ErrorMessage = "Vezetéknév mező kötelező")]
    [StringLength(30, ErrorMessage = "Vezetéknév mező 1-30 karakter hosszú lehet", MinimumLength = 1)]
    [RegularExpression(RegexPatterns.Name)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Keresztnév mező kötelező")]
    [StringLength(30, ErrorMessage = "Keresztnév mező 1-30 karakter hosszú lehet", MinimumLength = 1)]
    [RegularExpression(RegexPatterns.Name)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email cím mező kötelező")]
    [EmailAddress(ErrorMessage = "Email cím mező nem érvényes email cím")]
    [RegularExpression(RegexPatterns.Email)]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [RequiredIf("IsGoogleAccount", false, ErrorMessage = "Jelszó mező kötelező")]
    [StringLength(20, ErrorMessage = "Jelszó mező 6-20 karakter hosszú lehet", MinimumLength = 6)]
    [RegularExpression(RegexPatterns.Password)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [RequiredIf("IsGoogleAccount", false, ErrorMessage = "Jelszó megerősítése mező kötelező")]
    [StringLength(20, ErrorMessage = "Jelszó megerősítése mező 6-20 karakter hosszú lehet", MinimumLength = 6)]
    [Compare("Password", ErrorMessage = "Jelszó és Jelszó megerősítése mezőnek azonosnak kell lennie")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
}
