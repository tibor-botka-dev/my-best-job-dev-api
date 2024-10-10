using MyBestJob.DAL.Constants;
using MyBestJob.DAL.Database;
using MyBestJob.DAL.Database.Models;
using MongoFramework.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace MyBestJob.BLL.Services;

public interface IRoleService
{
    Task<Role?> GetRoleById(Guid id);
    Task<List<Role>> GetRoles();
    Task<List<string>> GetRolesByIds(List<Guid> ids);
    Task<List<Claim>> GetUserClaims(User user);
}

public class RoleService(MyBestJobDbContext context) : IRoleService
{
    private readonly MyBestJobDbContext _context = context;

    public async Task<List<Claim>> GetUserClaims(User user)
    {
        var roles = await GetRolesByIds(user.Roles);
        var currentUser = JsonSerializer.Serialize(new
        {
            email = user.Email,
            firstName = user.FirstName,
            lastName = user.LastName,
            fullName = user.FullName
        });

        var claims = new List<Claim>
        {
            new(Constants.ClaimTypes.Id, user.Id.ToString(), "Guid"),
            new(Constants.ClaimTypes.CurrentUser, currentUser)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }

    public async Task<Role?> GetRoleById(Guid id)
        => await _context.Roles.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<List<Role>> GetRoles()
        => await _context.Roles.ToListAsync();

    public async Task<List<string>> GetRolesByIds(List<Guid> ids)
        => await _context.Roles
            .Where(x => ids.Contains(x.Id))
            .Select(x => x.Name!)
            .ToListAsync();
}
