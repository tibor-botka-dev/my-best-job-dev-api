namespace MyBestJob.BLL.Exceptions;

public class UserExistsException(string email)
    : Exception($"User is already exists with same email: '{email}'")
{
    public string Email { get; } = email;
}