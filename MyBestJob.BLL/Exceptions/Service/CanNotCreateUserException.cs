using MyBestJob.BLL.Stuff;
using Microsoft.AspNetCore.Identity;

namespace MyBestJob.BLL.Exceptions;

public class CanNotCreateUserException(IEnumerable<IdentityError> errors, string? text = null)
    : Exception($"Error occured when creating user: {errors.GetErrorMessage()}. {text ?? string.Empty}")
{
    public IEnumerable<IdentityError> Errors { get; } = errors;
    public string? Text { get; } = text;
}
