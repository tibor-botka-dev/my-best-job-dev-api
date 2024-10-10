namespace MyBestJob.BLL.ViewModels;

public class GetEmailTemplateByTypeViewModel
{
    public GetEmailTemplateViewModel EmailTemplate { get; set; } = new();
    public List<GetEmailTemplateValueViewModel> EmailTemplateValues { get; set; } = [];
}
