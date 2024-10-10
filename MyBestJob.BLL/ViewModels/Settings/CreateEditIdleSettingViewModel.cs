using System.ComponentModel.DataAnnotations;
using static MyBestJob.DAL.Constants.Constants;

namespace MyBestJob.BLL.ViewModels;

public class CreateEditIdleSettingViewModel
{
    [Required(ErrorMessage = "Időtartam mező kötelező")]
    [RegularExpression(RegexPatterns.Duration)]
    public int Duration { get; set; }

    [RegularExpression(RegexPatterns.Duration)]
    public int Reminder { get; set; }

    [RegularExpression(RegexPatterns.Wait)]
    public int Wait { get; set; }

    [Required(ErrorMessage = "Háttérben mező kötelező")]
    public bool InBackground { get; set; }

    [Required(ErrorMessage = "Ismétlődés mező kötelező")]
    public bool Loop { get; set; }

    [Required(ErrorMessage = "Bekapcsolva mező kötelező")]
    public bool TurnedOn { get; set; }
}
