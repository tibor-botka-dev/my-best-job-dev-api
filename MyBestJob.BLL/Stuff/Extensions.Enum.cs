using MyBestJob.BLL.ViewModels;
using MyBestJob.DAL.Attributes;
using MyBestJob.DAL.Enums;

namespace MyBestJob.BLL.Stuff;

public static partial class Extensions
{
    public static EnumViewModel<T> GetEmailTemplateValueAttribute<T>(this T source) where T : struct
    {
        var fieldInfo = source.GetType().GetField(source.ToString() ?? "")
            ?? throw new NullReferenceException($"{nameof(source)} is null.");

        var attributes = fieldInfo.GetCustomAttributes(typeof(EnumViewModelAttribute), false) as EnumViewModelAttribute[]
            ?? throw new NullReferenceException($"{nameof(fieldInfo)} is null.");

        return new EnumViewModel<T>
        {
            Name = source,
            Key = attributes[0].Key ?? source.ToString()!,
            Value = attributes[0].Description
        };
    }

    public static List<EnumViewModel<T>> EnumToViewModelList<T>() where T : struct
    {
        if (!typeof(T).IsEnum)
            throw new InvalidCastException($"{nameof(T)} is not an enum type.");

        var result = Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select(x => x.GetEmailTemplateValueAttribute())
            .OrderBy(x => x.Value)
            .ToList();

        return result;
    }

    public static List<EnumViewModel<T>> EnumToViewModelList<T>(this List<T> source) where T : struct
    {
        if (!typeof(T).IsEnum)
            throw new InvalidCastException($"{nameof(T)} is not a list enum type.");

        var result = source
            .Select(x => x.GetEmailTemplateValueAttribute())
            .OrderBy(x => x.Value)
            .ToList();

        return result;
    }

    public static string GetFrontEndCallbackUrl(this Dictionary<RouteType, string> routes, RouteType routeType, params string[] args)
    {
        var route = routes[routeType];

        var parameters = string.Join('/', args);
        var callbackUrl = $"{route.TrimEnd('/')}/{parameters}".TrimEnd('/');

        return callbackUrl;
    }
}
