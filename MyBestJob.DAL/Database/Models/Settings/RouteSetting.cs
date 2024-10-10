using MyBestJob.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyBestJob.DAL.Database.Models;

public class RouteSetting
{
    [Required]
    public Dictionary<RouteType, string> Routes { get; set; } = [];
}
