using Microsoft.AspNetCore.Identity;

namespace MyBestJob.BLL.Stuff;

public static partial class Extensions
{
    public static string GetErrorMessage(this IEnumerable<IdentityError> errors)
    {
        var descriptions = errors.Select(error => error.Description);
        var errorMessage = string.Join("\n", descriptions);

        return errorMessage;
    }

    public static string GetExceptionMessages(this Exception exception)
    {
        var exceptions = GetAllException(exception);
        var exceptionMessages = exceptions.Select((e, i) => $"{i + 1}: {e.Message}");
        var messages = string.Join("\n", exceptionMessages);

        return messages;
    }

    private static IEnumerable<Exception> GetAllException(Exception exception)
    {
        do
        {
            yield return exception;
            exception = exception.InnerException!;
        } while (exception is not null);
    }
}

