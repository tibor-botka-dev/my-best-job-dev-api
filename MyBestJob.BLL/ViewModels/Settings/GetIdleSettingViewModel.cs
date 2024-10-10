namespace MyBestJob.BLL.ViewModels;

public class GetIdleSettingViewModel
{
    public Guid Id { get; set; }
    public Guid? CreatorUserId { get; set; }

    public int Duration { get; set; }
    public int Reminder { get; set; }
    public int Wait { get; set; }

    public bool InBackground { get; set; }
    public bool Loop { get; set; }
    public bool TurnedOn { get; set; }
}
