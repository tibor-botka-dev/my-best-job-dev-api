namespace MyBestJob.BLL.Exceptions;

public class UserNotFoundException(string? email = null)
    : Exception($"User not found: '{email}'")
{
    public string? Email { get; } = email;
}
