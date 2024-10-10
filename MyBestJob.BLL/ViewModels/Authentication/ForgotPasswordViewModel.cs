using System.ComponentModel.DataAnnotations;
using static MyBestJob.DAL.Constants.Constants;

namespace MyBestJob.BLL.ViewModels;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Email cím kötelező")]
    [EmailAddress(ErrorMessage = "Email cím nem érvényes email cím")]
    [RegularExpression(RegexPatterns.Email)]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;
}
