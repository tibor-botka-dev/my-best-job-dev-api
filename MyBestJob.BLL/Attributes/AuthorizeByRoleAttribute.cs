using MyBestJob.DAL.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace MyBestJob.BLL.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeByRoleAttribute : AuthorizeAttribute
{
    public AuthorizeByRoleAttribute(params string[] roles)
    {
        Roles = string.Join(",", roles.Length != 0 ? roles : DataSeeder.Roles.Select(x => x.Name));
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
    }
}
