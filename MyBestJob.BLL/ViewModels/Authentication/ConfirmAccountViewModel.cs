using System.ComponentModel.DataAnnotations;
using static MyBestJob.DAL.Constants.Constants;

namespace MyBestJob.BLL.ViewModels;

public class ConfirmAccountViewModel
{
    [Required(ErrorMessage = "Email cím mező kötelező")]
    [EmailAddress(ErrorMessage = "Email cím mező nem érvényes email cím")]
    [RegularExpression(RegexPatterns.Email)]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Token kötelező")]
    public string Token { get; set; } = string.Empty;
}
