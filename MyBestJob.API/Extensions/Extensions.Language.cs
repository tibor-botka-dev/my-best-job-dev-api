using MyBestJob.DAL.Database.Models;
using System.Globalization;

namespace MyBestJob.API.Extensions;

public static partial class Extensions
{
    public static List<CultureInfo> GetSupportedCultures(this IEnumerable<Language> languages)
        => languages
            .Select(x => new CultureInfo(x.ExtendedName))
            .ToList();
}
