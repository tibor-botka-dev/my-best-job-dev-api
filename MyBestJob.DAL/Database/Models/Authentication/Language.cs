using System.ComponentModel.DataAnnotations;

namespace MyBestJob.DAL.Database.Models;

public class Language : BaseModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string ExtendedName { get; set; } = string.Empty;
}
