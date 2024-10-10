namespace MyBestJob.BLL.Exceptions;

public class MissingSettingException(string appSettingName)
    : Exception($"'{appSettingName}' is unavailable from appsettings file.")
{
    public string AppSettingName { get; } = appSettingName;
}
