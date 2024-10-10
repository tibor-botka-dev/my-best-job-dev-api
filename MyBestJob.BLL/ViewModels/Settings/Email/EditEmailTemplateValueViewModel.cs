using System.ComponentModel.DataAnnotations;

namespace MyBestJob.BLL.ViewModels;

public class EditEmailTemplateValueViewModel
{
    [Required(ErrorMessage = "Változó neve mező kötelező")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Változó értéke mező kötelező")]
    public string Value { get; set; } = string.Empty;
}
