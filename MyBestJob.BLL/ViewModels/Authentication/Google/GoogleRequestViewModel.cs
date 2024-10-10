using System.ComponentModel.DataAnnotations;

namespace MyBestJob.BLL.ViewModels;

public class GoogleRequestViewModel
{
    [Required]
    public string Code { get; set; } = string.Empty;

    public string? Authuser { get; set; }

    public string? Prompt { get; set; }

    public string? Scope { get; set; }

    public string? State { get; set; }
}
