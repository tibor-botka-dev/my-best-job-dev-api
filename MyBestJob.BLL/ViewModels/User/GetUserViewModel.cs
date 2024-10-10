namespace MyBestJob.BLL.ViewModels;

public class GetUserViewModel
{
    public Guid Id { get; set; }

    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Avatar { get; set; }

    public List<Guid> Roles { get; set; } = [];
}
