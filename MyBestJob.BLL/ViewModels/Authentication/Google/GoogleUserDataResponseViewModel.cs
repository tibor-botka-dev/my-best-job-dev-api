using Newtonsoft.Json;

namespace MyBestJob.BLL.ViewModels;

public class GoogleUserDataResponseViewModel
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("verified_email")]
    public string VerifiedEmail { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("given_name")]
    public string GivenName { get; set; } = string.Empty;

    [JsonProperty("family_name")]
    public string FamilyName { get; set; } = string.Empty;

    [JsonProperty("picture")]
    public string Picture { get; set; } = string.Empty;

    [JsonProperty("locale")]
    public string Locale { get; set; } = string.Empty;
}
