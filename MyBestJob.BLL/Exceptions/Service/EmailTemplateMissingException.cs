using MyBestJob.DAL.Enums;

namespace MyBestJob.BLL.Exceptions;

public class EmailTemplateMissingException(EmailTemplateType emailTemplateType)
    : Exception($"'{emailTemplateType}' email template type is missing.")
{
    public EmailTemplateType EmailTemplateType { get; } = emailTemplateType;
}
