using Newtonsoft.Json;

namespace MyBestJob.BLL.ViewModels;

public class GoogleUrlViewModel
{
    [JsonProperty("redirect_uri")]
    public string RedirectUri { get; set; } = string.Empty;

    [JsonProperty("client_id")]
    public string ClientId { get; set; } = string.Empty;

    [JsonProperty("access_type")]
    public string AccessType { get; set; } = string.Empty;

    [JsonProperty("response_type")]
    public string ResponseType { get; set; } = string.Empty;

    [JsonProperty("prompt")]
    public string Prompt { get; set; } = string.Empty;

    [JsonProperty("scope")]
    public string Scope { get; set; } = string.Empty;

    [JsonProperty("state")]
    public string State { get; set; } = string.Empty;
}
