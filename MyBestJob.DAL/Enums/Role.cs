using System.ComponentModel;

namespace MyBestJob.DAL.Enums;

public enum Role
{
    [Description("Adminisztrátor")]
    Administrator,
    [Description("Felhasználó")]
    User,
    [Description("Támogató")]
    Support,
    [Description("Megfigyelő")]
    Spectator,
}
