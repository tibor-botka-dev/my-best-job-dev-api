using MyBestJob.BLL.Exceptions;
using Microsoft.AspNetCore.Http;
using static MyBestJob.DAL.Constants.Constants;

namespace MyBestJob.BLL.Stuff;

public static partial class Extensions
{
    public static string ValidateAndGetAvatarAsBase64(this IFormFile file)
    {
        if (file.Length > 1024 * 1024 * Avatar.SizeInMegaByte
            || !file.ContentType.StartsWith("image/"))
            throw new InvalidFileException(file.Length / 1024 / 1024, file.ContentType);

        var bytes = GetFileDataAsBytes(file);

        return $"data:image/png;base64, {Convert.ToBase64String(bytes)}";
    }

    private static byte[] GetFileDataAsBytes(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);

        return memoryStream.ToArray();
    }
}
