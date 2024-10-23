using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MyBestJob.BLL.Stuff;

public static partial class Extensions
{
    public static void Fatal<T>(this ILogger<T> logger, string message, params string[] args)
        => WriteWithMessage(LogLevel.Critical, logger, message, args);
    public static void Error<T>(this ILogger<T> logger, string message, params string[] args)
        => WriteWithMessage(LogLevel.Error, logger, message, args);
    public static void Warning<T>(this ILogger<T> logger, string message, params string[] args)
        => WriteWithMessage(LogLevel.Warning, logger, message, args);
    public static void Info<T>(this ILogger<T> logger, string message, params string[] args)
        => WriteWithMessage(LogLevel.Information, logger, message, args);
    public static void Debug<T>(this ILogger<T> logger, string message, params string[] args)
        => WriteWithMessage(LogLevel.Debug, logger, message, args);
    public static void Trace<T>(this ILogger<T> logger, string? message = null, params string[] args)
        => WriteWithMessage(LogLevel.Trace, logger, message, args);
    public static void Trace<T>(this ILogger<T> logger, string? message = null, object? objectToSerialize = null, params string[] args)
        => WriteWithMessageAndObject(LogLevel.Trace, logger, message, objectToSerialize, args);

    public static void Fatal<T>(this ILogger<T> logger, Exception exception, string? message = null, params string[] args)
        => WriteWithException(LogLevel.Critical, logger, exception, message, args);
    public static void Error<T>(this ILogger<T> logger, Exception exception, string? message = null, params string[] args)
        => WriteWithException(LogLevel.Error, logger, exception, message, args);
    public static void Warning<T>(this ILogger<T> logger, Exception exception, string? message = null, params string[] args)
        => WriteWithException(LogLevel.Warning, logger, exception, message, args);
    public static void Info<T>(this ILogger<T> logger, Exception exception, string? message = null, params string[] args)
        => WriteWithException(LogLevel.Information, logger, exception, message, args);
    public static void Debug<T>(this ILogger<T> logger, Exception exception, string? message = null, params string[] args)
        => WriteWithException(LogLevel.Debug, logger, exception, message, args);
    public static void Trace<T>(this ILogger<T> logger, Exception exception, string? message = null, params string[] args)
        => WriteWithException(LogLevel.Trace, logger, exception, message, args);

    private static void WriteWithMessage<T>(
        LogLevel level,
        ILogger<T> logger,
        string? message = null,
        params string[] args)
    {
        logger.Log(level, message, args);
    }

    private static void WriteWithMessageAndObject<T>(
        LogLevel level,
        ILogger<T> logger,
        string? message = null,
        object? objectToSerialize = null,
        params string[] args)
    {
        logger.Log(level, $"{message ?? "Object serialized: "}{JsonSerializer.Serialize(objectToSerialize)}", args);
    }

    private static void WriteWithException<T>(
        LogLevel level,
        ILogger<T> logger,
        Exception exception,
        string? message = null,
        params string[] args)
    {
        logger.Log(level, exception, message, args);
    }
}
