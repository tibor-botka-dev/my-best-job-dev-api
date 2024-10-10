using Newtonsoft.Json;

namespace MyBestJob.BLL.ViewModels;

public class GoogleUserDataRequestViewModel
{
    [JsonProperty("alt")]
    public string Alt { get; set; } = string.Empty;

    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = string.Empty;
}
