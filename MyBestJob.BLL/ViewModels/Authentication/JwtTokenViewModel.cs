namespace MyBestJob.BLL.ViewModels;

public class JwtTokenViewModel
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpires { get; set; }

    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpires { get; set; }
}
