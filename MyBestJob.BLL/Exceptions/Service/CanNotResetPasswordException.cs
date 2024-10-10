using MyBestJob.BLL.Stuff;
using Microsoft.AspNetCore.Identity;

namespace MyBestJob.BLL.Exceptions;

public class CanNotResetPasswordException(IEnumerable<IdentityError> errors, string email)
    : Exception($"Can not reset password with email: '{email}'.\nErrors: '{errors.GetErrorMessage()}'")
{
    public IEnumerable<IdentityError> Errors { get; } = errors;
    public string Email { get; set; } = email;
}
