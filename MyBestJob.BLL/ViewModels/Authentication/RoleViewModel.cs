namespace MyBestJob.BLL.ViewModels.Authentication;

public class RoleViewModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool HasUserAccess { get; set; }
}
