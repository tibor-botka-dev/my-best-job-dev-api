using MyBestJob.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyBestJob.DAL.Database.Models;

public class MailSetting : BaseModel
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    public string AesPassword { get; set; } = string.Empty;

    [Required]
    public string AesKey { get; set; } = string.Empty;

    [Required]
    public string Host { get; set; } = string.Empty;

    [Required]
    public int Port { get; set; }

    [Required]
    public List<EmailTemplate> EmailTemplates { get; set; } = [];

    [Required]
    public List<EmailTemplateValue> EmailTemplateValues { get; set; } = [];
}

public class EmailTemplate
{
    [Required]
    public EmailTemplateType EmailTemplateType { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string HtmlTemplate { get; set; } = string.Empty;
}

public class EmailTemplateValue
{
    [Required]
    public bool IsRequired { get; set; } = true;

    [Required]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    public EmailTemplateValueType? EmailTemplateValueType { get; set; }

    public string? Value { get; set; }
}