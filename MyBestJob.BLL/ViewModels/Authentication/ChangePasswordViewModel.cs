using System.ComponentModel.DataAnnotations;
using static MyBestJob.DAL.Constants.Constants;

namespace MyBestJob.BLL.ViewModels;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Régi jelszó mező kötelező")]
    [StringLength(20, ErrorMessage = "Régi jelszó mező 6-20 karakter hosszú lehet", MinimumLength = 6)]
    [RegularExpression(RegexPatterns.Password)]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Új jelszó mező kötelező")]
    [StringLength(20, ErrorMessage = "Új jelszó mező 6-20 karakter hosszú lehet", MinimumLength = 6)]
    [Compare("ConfirmPassword", ErrorMessage = "Új jelszó és Új jelszó megerősítése mezőnek azonosnak kell lennie")]
    [RegularExpression(RegexPatterns.Password)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Új jelszó megerősítése mező kötelező")]
    [StringLength(20, ErrorMessage = "Új jelszó megerősítése mező 6-20 karakter hosszú lehet", MinimumLength = 6)]
    [Compare("Password", ErrorMessage = "Új jelszó és Új jelszó megerősítése mezőnek azonosnak kell lennie")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
}
