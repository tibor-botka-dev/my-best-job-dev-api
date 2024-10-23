using MyBestJob.API.Extensions;
using MyBestJob.DAL.Database;
using System.Globalization;
using static MyBestJob.DAL.Constants.Constants;

namespace MyBestJob.API.Middleware;

public class LocalizationMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        var acceptLanguageHeader = context
            .Request
            .Headers[DefaultLanguages.AcceptLanguageHeader]
            .ToString();

        if (!string.IsNullOrEmpty(acceptLanguageHeader))
        {
            var supportedCultures = DataSeeder.Languages.GetSupportedCultures();

            var cultureExists = supportedCultures.Any(x => acceptLanguageHeader.Contains(x.Name, StringComparison.OrdinalIgnoreCase))
                || supportedCultures.Any(x => string.Equals(x.Name, acceptLanguageHeader, StringComparison.OrdinalIgnoreCase));
            if (!cultureExists)
                await _next(context);

            var currentCulture = new CultureInfo(acceptLanguageHeader);
            Thread.CurrentThread.CurrentCulture = currentCulture;
            Thread.CurrentThread.CurrentUICulture = currentCulture;
        }

        await _next(context);
    }
}
