using MyBestJob.DAL.Enums;

namespace MyBestJob.BLL.Stuff;

public static partial class Extensions
{
    public static string GetFrontEndCallbackUrl(this Dictionary<RouteType, string> routes, RouteType routeType, params string[] args)
    {
        var route = routes[routeType];

        var parameters = string.Join('/', args);
        var callbackUrl = $"{route.TrimEnd('/')}/{parameters}".TrimEnd('/');

        return callbackUrl;
    }
}
