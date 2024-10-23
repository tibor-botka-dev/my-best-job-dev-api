namespace MyBestJob.BLL.Stuff;

public static partial class Extensions
{
    public static string ReplaceHtmlTemplate(this string htmlTemplate, string key, string value)
    => htmlTemplate.Replace($"--{key}--", value);
}
