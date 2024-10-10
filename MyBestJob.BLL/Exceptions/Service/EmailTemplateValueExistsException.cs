namespace MyBestJob.BLL.Exceptions;

public class EmailTemplateValueExistsException(string key)
    : Exception($"Email template value is already exists with same key: '{key}'")
{
    public string Key { get; } = key;
}