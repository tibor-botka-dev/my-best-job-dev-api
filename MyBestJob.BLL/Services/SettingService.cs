using AutoMapper;
using MyBestJob.BLL.Exceptions;
using MyBestJob.BLL.ViewModels;
using MyBestJob.DAL.Database;
using MyBestJob.DAL.Database.Models;
using MyBestJob.DAL.Enums;
using Microsoft.Extensions.Options;
using MongoFramework.Linq;

namespace MyBestJob.BLL.Services;

public interface ISettingService
{
    Task<GetMailSettingViewModel> GetMailSetting();
    Task<GetEmailTemplatesViewModel> GetEmailTemplates();
    Task<GetEmailTemplateByTypeViewModel> GetEmailTemplateByType(EmailTemplateType emailTemplateType);

    Task UpdateEmailTemplate(EmailTemplateType emailTemplateType, EditEmailTemplateViewModel viewModel);
    Task UpdateMailSetting(EditMailSettingViewModel viewModel, Guid userId);

    Task CreateOrUpdateIdleSetting(CreateEditIdleSettingViewModel viewModel, Guid userId);

    Task CreateEmailTemplateValue(CreateEmailTemplateValueViewModel viewModel);
    Task UpdateEmailTemplateValue(string key, EditEmailTemplateValueViewModel viewModel);
    Task DeleteEmailTemplateValue(string key);
}

public class SettingService(MyBestJobDbContext context,
    IMapper mapper,
    IOptions<MailSetting> mailSetting) : ISettingService
{
    private readonly MyBestJobDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly MailSetting _mailSetting = mailSetting.Value;

    public async Task<GetMailSettingViewModel> GetMailSetting()
    {
        var mailSetting = await _context.MailSettings.FirstOrDefaultAsync()
            ?? throw new MissingSettingException(nameof(MailSetting));
        var viewModel = _mapper.Map<GetMailSettingViewModel>(mailSetting);

        return viewModel;
    }

    public async Task<GetEmailTemplatesViewModel> GetEmailTemplates()
    {
        var mailSetting = await _context.MailSettings.FirstOrDefaultAsync()
            ?? throw new MissingSettingException(nameof(MailSetting));
        var viewModel = _mapper.Map<GetEmailTemplatesViewModel>(mailSetting);

        return viewModel;
    }

    public async Task<GetEmailTemplateByTypeViewModel> GetEmailTemplateByType(EmailTemplateType emailTemplateType)
    {
        var mailSetting = await _context.MailSettings.FirstOrDefaultAsync()
            ?? throw new MissingSettingException(nameof(MailSetting));

        var emailTemplate = mailSetting.EmailTemplates.FirstOrDefault(x => x.EmailTemplateType == emailTemplateType)
            ?? throw new EmailTemplateMissingException(emailTemplateType);

        var viewModel = _mapper.Map<GetEmailTemplateByTypeViewModel>(mailSetting);
        viewModel.EmailTemplate = _mapper.Map<GetEmailTemplateViewModel>(emailTemplate);

        return viewModel;
    }

    public async Task CreateOrUpdateIdleSetting(CreateEditIdleSettingViewModel viewModel, Guid userId)
    {
        var idleSetting = await _context.IdleSettings.FirstOrDefaultAsync(x => x.CreatedByUserId == userId)
            ?? await _context.IdleSettings.FirstOrDefaultAsync(x => x.CreatedByUserId == null)
            ?? throw new MissingSettingException(nameof(IdleSetting));

        if (idleSetting.CreatedByUserId == null)
        {
            idleSetting = _mapper.Map<IdleSetting>(viewModel);
            idleSetting.CreatedByUserId = userId;

            _context.IdleSettings.Add(idleSetting);
        }
        else
        {
            _mapper.Map(viewModel, idleSetting);
            idleSetting.UpdatedByUserId = userId;
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateMailSetting(EditMailSettingViewModel viewModel, Guid userId)
    {
        var mailSetting = await _context.MailSettings.FirstOrDefaultAsync()
            ?? throw new MissingSettingException(nameof(MailSetting));

        _mapper.Map(viewModel, mailSetting);
        mailSetting.UpdatedByUserId = userId;

        await _context.SaveChangesAsync();
    }

    public async Task UpdateEmailTemplate(EmailTemplateType emailTemplateType, EditEmailTemplateViewModel viewModel)
    {
        var mailSetting = await _context.MailSettings.FirstOrDefaultAsync()
            ?? throw new MissingSettingException(nameof(MailSetting));

        var emailTemplate = mailSetting.EmailTemplates.FirstOrDefault(x => x.EmailTemplateType == emailTemplateType)
            ?? throw new MissingSettingException(nameof(EmailTemplate));

        _mapper.Map(viewModel, emailTemplate);

        await _context.SaveChangesAsync();
    }

    public async Task CreateEmailTemplateValue(CreateEmailTemplateValueViewModel viewModel)
    {
        var mailSetting = await _context.MailSettings.FirstOrDefaultAsync()
            ?? throw new MissingSettingException(nameof(MailSetting));

        if (mailSetting.EmailTemplateValues.Any(x => x.Key == viewModel.Key))
            throw new EmailTemplateValueExistsException(viewModel.Key);

        var emailTemplateValue = _mapper.Map<EmailTemplateValue>(viewModel);

        mailSetting.EmailTemplateValues.Add(emailTemplateValue);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateEmailTemplateValue(string key, EditEmailTemplateValueViewModel viewModel)
    {
        var mailSetting = await _context.MailSettings.FirstOrDefaultAsync()
            ?? throw new MissingSettingException(nameof(MailSetting));

        var emailTemplateValue = mailSetting.EmailTemplateValues.FirstOrDefault(x => x.Key == key && !x.IsRequired)
            ?? throw new MissingSettingException(nameof(EmailTemplateValue));

        _mapper.Map(viewModel, emailTemplateValue);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteEmailTemplateValue(string key)
    {
        var mailSetting = await _context.MailSettings.FirstOrDefaultAsync()
            ?? throw new MissingSettingException(nameof(MailSetting));

        if (!mailSetting.EmailTemplateValues.Any(x => x.Key == key && !x.IsRequired))
            throw new MissingSettingException(nameof(EmailTemplateValue));

        mailSetting.EmailTemplateValues.RemoveAll(x => x.Key == key);

        await _context.SaveChangesAsync();
    }
}
