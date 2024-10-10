using Newtonsoft.Json;

namespace MyBestJob.BLL.ViewModels;

public class GoogleTokenResponseViewModel
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonProperty("id_token")]
    public string IdToken { get; set; } = string.Empty;

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;

    [JsonProperty("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonProperty("scope")]
    public string Scope { get; set; } = string.Empty;
}
