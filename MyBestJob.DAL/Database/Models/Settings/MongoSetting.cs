using System.ComponentModel.DataAnnotations;

namespace MyBestJob.DAL.Database.Models;

public class MongoSetting
{
    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    [Required]
    public string Database { get; set; } = string.Empty;
}
