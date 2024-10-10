using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using static MyBestJob.DAL.Constants.Constants;

namespace MyBestJob.BLL.ViewModels;

public class CreateUserViewModel
{
    [Required(ErrorMessage = "Szerepkör kiválasztása kötelező")]
    public List<Guid> Roles { get; set; } = [];

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

    public IFormFile? Avatar { get; set; }
}
