namespace MyBestJob.BLL.ViewModels;

public class SignInResponseViewModel
{
    public JwtTokenViewModel Tokens { get; set; } = new();

    public string? Avatar { get; set; }
}
