using System.ComponentModel.DataAnnotations;

namespace MyBestJob.BLL.ViewModels;

public class CreateEmailTemplateValueViewModel : EditEmailTemplateValueViewModel
{
    [Required(ErrorMessage = "Változó 'kulcs' mező kötelező")]
    public string Key { get; set; } = string.Empty;
}
