namespace MyBestJob.BLL.ViewModels;

public class EnumViewModel<T> where T : struct
{
    public T Name { get; set; }

    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
