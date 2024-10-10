using System.ComponentModel.DataAnnotations;

namespace MyBestJob.BLL.ViewModels;

public class SignOutViewModel
{
    [Required(ErrorMessage = "Access token kötelező")]
    public string AccessToken { get; set; } = string.Empty;
}
