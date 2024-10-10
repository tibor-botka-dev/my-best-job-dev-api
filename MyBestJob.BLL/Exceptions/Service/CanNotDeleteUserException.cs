using MyBestJob.BLL.Stuff;
using Microsoft.AspNetCore.Identity;

namespace MyBestJob.BLL.Exceptions.Service;

public class CanNotDeleteUserException(IEnumerable<IdentityError> errors)
    : Exception($"Error occured when deleting user: {errors.GetErrorMessage()}")
{
    public IEnumerable<IdentityError> Errors { get; } = errors;
}
