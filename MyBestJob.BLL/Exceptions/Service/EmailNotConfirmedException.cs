namespace MyBestJob.BLL.Exceptions;

public class EmailNotConfirmedException(string email)
    : Exception($"Email is not confirmed yet: '{email}'")
{
    public string Email { get; } = email;
}
