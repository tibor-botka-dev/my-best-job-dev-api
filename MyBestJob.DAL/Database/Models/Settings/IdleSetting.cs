using System.ComponentModel.DataAnnotations;

namespace MyBestJob.DAL.Database.Models;

public class IdleSetting : BaseModel
{
    [Required]
    public int Duration { get; set; }

    [Required]
    public int Reminder { get; set; }

    [Required]
    public int Wait { get; set; }

    [Required]
    public bool Loop { get; set; }

    [Required]
    public bool InBackground { get; set; }

    [Required]
    public bool TurnedOn { get; set; }
}
