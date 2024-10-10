namespace MyBestJob.DAL.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class EnumViewModelAttribute(string description, string? key = null) : Attribute
{
    public string Description { get; private set; } = description;
    public string? Key { get; private set; } = key;
}
