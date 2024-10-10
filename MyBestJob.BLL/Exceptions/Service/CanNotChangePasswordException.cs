using MyBestJob.BLL.Stuff;
using Microsoft.AspNetCore.Identity;

namespace MyBestJob.BLL.Exceptions;

public class CanNotChangePasswordException(IEnumerable<IdentityError> errors, string email)
    : Exception($"Can not change password with email: '{email}'.\nErrors: '{errors.GetErrorMessage()}'")
{
    public IEnumerable<IdentityError> Errors { get; } = errors;
    public string Email { get; set; } = email;
}
