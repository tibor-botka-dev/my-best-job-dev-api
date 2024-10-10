using System.ComponentModel.DataAnnotations;
using static MyBestJob.DAL.Constants.Constants;

namespace MyBestJob.BLL.ViewModels;

public class ResetPasswordViewModel : ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Jelszó mező kötelező")]
    [StringLength(20, ErrorMessage = "Jelszó mező 6-20 karakter hosszú lehet", MinimumLength = 6)]
    [Compare("ConfirmPassword", ErrorMessage = "Jelszó és Jelszó megerősítése mezőnek azonosnak kell lennie")]
    [RegularExpression(RegexPatterns.Password)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Jelszó megerősítése mező kötelező")]
    [StringLength(20, ErrorMessage = "Jelszó megerősítése mező 6-20 karakter hosszú lehet", MinimumLength = 6)]
    [Compare("Password", ErrorMessage = "Jelszó és Jelszó megerősítése mezőnek azonosnak kell lennie")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Token kötelező")]
    public string Token { get; set; } = string.Empty;
}
