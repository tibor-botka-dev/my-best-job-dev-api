using System.ComponentModel.DataAnnotations;

namespace MyBestJob.DAL.Database.Models;

public class AdminSetting
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;
}

