namespace MyBestJob.BLL.Exceptions;

public class CanNotSendEmailException(string email, string message)
    : Exception(message)
{
    public string Email { get; } = email;
}
