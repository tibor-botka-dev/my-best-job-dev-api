using MyBestJob.DAL.Database.Models;
using static MyBestJob.DAL.Constants.Constants;
using RoleEnum = MyBestJob.DAL.Enums.Role;

namespace MyBestJob.DAL.Database;

public static class DataSeeder
{
    public static IEnumerable<Role> Roles
        => [
                new() {
                    Id = DefaultRoles.AdministratorRoleId,
                    Name = nameof(RoleEnum.Administrator),
                    NormalizedName = nameof(RoleEnum.Administrator).ToUpper() },
                new() {
                    Id = DefaultRoles.UserRoleId,
                    Name = nameof(RoleEnum.User),
                    NormalizedName = nameof(RoleEnum.User).ToUpper() },
                new() {
                    Id = DefaultRoles.WebsiteSupporterRoleId,
                    Name = nameof(RoleEnum.Support),
                    NormalizedName = nameof(RoleEnum.Support).ToUpper() },
                new() {
                    Id = DefaultRoles.SpectatorRoleId,
                    Name = nameof(RoleEnum.Spectator),
                    NormalizedName = nameof(RoleEnum.Spectator).ToUpper() },
            ];
}
