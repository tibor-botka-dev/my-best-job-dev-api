using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Globalization;

namespace MyBestJob.API.Localizers;

public class JsonStringLocalizer(IDistributedCache cache) : IStringLocalizer
{
    private readonly IDistributedCache _cache = cache;

    public LocalizedString this[string key]
        => GetValue(key);

    public LocalizedString this[string key, params object[] arguments]
        => GetValue(key, arguments);

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var language = CultureInfo.CurrentCulture.Name;

        var localizations = ReadJson(language);
        var result = localizations!
            .Select(x => new LocalizedString(x.Key, x.Value, false));

        return result;
    }

    private LocalizedString GetValue(string key)
    {
        var language = CultureInfo.CurrentCulture.Name;

        var cacheKey = $"locale_{language}_{key}";
        var cacheValue = _cache.GetString(cacheKey);
        if (!string.IsNullOrEmpty(cacheValue))
            return new LocalizedString(key, cacheValue, false);

        var localizations = ReadJson(language);
        if (localizations == null || !localizations.ContainsKey(key))
            return new LocalizedString(key, key, false);

        var localization = localizations.GetValueOrDefault(key, key);
        _cache.SetString(cacheKey, localization);

        return new LocalizedString(key, localization, false);
    }

    private LocalizedString GetValue(string key, params object[] arguments)
    {
        var localization = GetValue(key);

        return new LocalizedString(key, string.Format(localization.Value, arguments), false);
    }

    private static Dictionary<string, string>? ReadJson(string language)
    {
        var filePath = $"Localizations/Locales.{language}.json";
        if (!File.Exists(filePath))
            return null;

        using var streamReader = new StreamReader(filePath);
        var json = streamReader.ReadToEnd();
        var localizations = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

        return localizations;
    }
}