using MyBestJob.BLL.Exceptions;
using MyBestJob.DAL.Constants;
using System.Security.Claims;

namespace MyBestJob.BLL.Stuff;

public static partial class Extensions
{
    public static async Task<Guid?> GetId(this IEnumerable<Claim> userClaims)
    {
        var idClaim = userClaims.FirstOrDefault(x => x.Type == Constants.ClaimTypes.Id);
        return idClaim == null
            ? null : !Guid.TryParse(idClaim.Value, out Guid id)
            ? null : await Task.FromResult(id);
    }

    public static async Task<Guid> GetRequiredId(this IEnumerable<Claim> userClaims)
    {
        var id = await GetId(userClaims)
            ?? throw new UserNotSignedInException("User not logged in: Id claim is missing.");

        return id;
    }
}
