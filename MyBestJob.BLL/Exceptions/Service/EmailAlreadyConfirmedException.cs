namespace MyBestJob.BLL.Exceptions;

public class EmailAlreadyConfirmedException(string email, string message)
    : Exception(message)
{
    public string Email { get; } = email;
}
