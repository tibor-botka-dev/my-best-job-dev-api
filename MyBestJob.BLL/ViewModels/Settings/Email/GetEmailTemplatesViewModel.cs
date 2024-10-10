using MyBestJob.DAL.Enums;

namespace MyBestJob.BLL.ViewModels;

public class GetEmailTemplatesViewModel
{
    public List<GetEmailTemplateViewModel> EmailTemplates { get; set; } = [];
    public List<GetEmailTemplateValueViewModel> EmailTemplateValues { get; set; } = [];
}

public class GetEmailTemplateViewModel
{
    public EmailTemplateType EmailTemplateType { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string HtmlTemplate { get; set; } = string.Empty;
}

public class GetEmailTemplateValueViewModel
{
    public bool IsRequired { get; set; } = true;

    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public EmailTemplateValueType? EmailTemplateValueType { get; set; }

    public string? Value { get; set; }
}