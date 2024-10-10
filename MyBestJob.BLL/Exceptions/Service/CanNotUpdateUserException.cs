using MyBestJob.BLL.Stuff;
using Microsoft.AspNetCore.Identity;

namespace MyBestJob.BLL.Exceptions;

public class CanNotUpdateUserException(IEnumerable<IdentityError> errors)
    : Exception($"Error occured when updating user: {errors.GetErrorMessage()}")
{
    public IEnumerable<IdentityError> Errors { get; } = errors;
}
