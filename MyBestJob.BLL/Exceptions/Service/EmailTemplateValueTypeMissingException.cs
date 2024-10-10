using MyBestJob.DAL.Enums;

namespace MyBestJob.BLL.Exceptions;

public class EmailTemplateValueTypeMissingException(EmailTemplateValueType emailTemplateValueType)
    : Exception($"'{emailTemplateValueType}' email template value type is missing")
{
    public EmailTemplateValueType EmailTemplateValueType { get; } = emailTemplateValueType;
}
