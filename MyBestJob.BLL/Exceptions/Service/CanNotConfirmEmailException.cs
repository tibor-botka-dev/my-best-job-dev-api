using MyBestJob.BLL.Stuff;
using Microsoft.AspNetCore.Identity;

namespace MyBestJob.BLL.Exceptions;

public class CanNotConfirmEmailException(IEnumerable<IdentityError> errors)
    : Exception($"Error occured when confirm email: '{errors.GetErrorMessage()}'")
{
    public IEnumerable<IdentityError> Errors { get; } = errors;
}
