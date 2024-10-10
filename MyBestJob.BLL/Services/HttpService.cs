using MyBestJob.BLL.Exceptions;
using MyBestJob.BLL.Stuff;
using MyBestJob.DAL.Enums;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;

namespace MyBestJob.BLL.Services;

public interface IHttpService
{
    Task<HttpClientResponse<T>> Get<T>(string url,
        object? request = null,
        HttpRequestType? requestType = HttpRequestType.Json)
        where T : class?;
    Task<HttpClientResponse<T>> Post<T>(string url,
        object? request = null,
        HttpRequestType? requestType = HttpRequestType.Json)
    where T : class?;

    string BaseUrl { get; set; }
    string? Token { get; set; }
    Dictionary<string, string> Headers { get; set; }
    JsonSerializerSettings JsonSerializerSettings { get; set; }
}

public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpService> _logger;

    public string BaseUrl { get; set; } = string.Empty;
    public string? Token { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public JsonSerializerSettings JsonSerializerSettings { get; set; }

    public HttpService(IHttpClientFactory httpClientFactory,
        ILogger<HttpService> logger)
    {
        _httpClient = httpClientFactory.CreateClient(nameof(HttpClient));
        _logger = logger;

        JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
    }

    public async Task<HttpClientResponse<T>> Get<T>(string url,
        object? request = null,
        HttpRequestType? requestType = HttpRequestType.Json) where T : class?
    {
        _logger.Trace(url);

        var requestMessage = GetRequestMessageWithJsonBody(HttpMethod.Get, url, request, requestType);
        var response = await Send<T>(requestMessage);

        return response;
    }

    public async Task<HttpClientResponse<T>> Post<T>(string url,
        object? request = null,
        HttpRequestType? requestType = HttpRequestType.Json) where T : class?
    {
        _logger.Trace(url);

        var requestMessage = GetRequestMessageWithJsonBody(HttpMethod.Post, url, request, requestType);
        var response = await Send<T>(requestMessage);

        return response;
    }

    private async Task<HttpClientResponse<T>> Send<T>(HttpRequestMessage requestMessage)
        where T : class?
    {
        try
        {
            var responseMessage = await _httpClient.SendAsync(requestMessage);

            var responseBody = await responseMessage.Content.ReadAsStringAsync();

            if (!responseMessage.IsSuccessStatusCode)
            {
                _logger.Warning($"HTTP request is unsuccessful with status code: {(int)responseMessage.StatusCode}," +
                    $" with method: {responseMessage.RequestMessage?.Method}," +
                    $" with url: {responseMessage.RequestMessage?.RequestUri}," +
                    $" with body: {responseBody}.");

                var errorObject = default(JObject);
                try
                {
                    errorObject = JObject.Parse(responseBody);
                }
                catch { }

                return new HttpClientResponse<T>
                {
                    StatusCode = responseMessage.StatusCode,
                    IsSuccessStatusCode = responseMessage.IsSuccessStatusCode,
                    ResponseString = responseMessage.ReasonPhrase,
                    ErrorObject = errorObject
                };
            }

            var result = JsonConvert.DeserializeObject<T>(responseBody, JsonSerializerSettings);
            if (result is null)
                _logger.Error($"Can not deserialize to '{typeof(T).Name}' response body: {responseBody}.");

            return new HttpClientResponse<T>
            {
                StatusCode = responseMessage.StatusCode,
                IsSuccessStatusCode = responseMessage.IsSuccessStatusCode,
                Object = result
            };
        }
        catch (Exception ex)
        {
            _logger.Trace(ex.GetExceptionMessages());
            throw new HttpErrorException(ex.Message);
        }
    }

    private HttpRequestMessage GetRequestMessageWithJsonBody(HttpMethod method,
        string url,
        object? request,
        HttpRequestType? requestType)
    {
        var requestMessage = GetRequestMessage(method, url);

        if (request != null)
        {
            var jsonRequest = JsonConvert.SerializeObject(request);
            switch (requestType)
            {
                case HttpRequestType.Json:
                    requestMessage.Content = new StringContent(jsonRequest,
                        Encoding.UTF8,
                        MediaTypeNames.Application.Json);
                    break;
                case HttpRequestType.FormUrlEncodedContent:
                    var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                        jsonRequest);
                    requestMessage.Content = new FormUrlEncodedContent(
                        dictionary ?? new Dictionary<string, string>());
                    break;
                default:
                    break;
            }
        }

        return requestMessage;
    }

    private HttpRequestMessage GetRequestMessage(HttpMethod method, string url)
    {
        var requestMessage = new HttpRequestMessage(method, $"{BaseUrl.TrimEnd('/')}/{url}");

        if (!string.IsNullOrEmpty(Token))
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);

        foreach (var keyValuePair in Headers)
            requestMessage.Headers.Add(keyValuePair.Key, keyValuePair.Value);

        return requestMessage;
    }
}

public class HttpClientResponse<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccessStatusCode { get; set; }
    public T? Object { get; set; }
    public JObject? ErrorObject { get; set; }
    public string? ResponseString { get; set; }
}