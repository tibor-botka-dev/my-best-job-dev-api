namespace MyBestJob.BLL.Exceptions;

public class IncorrectPasswordException(string message)
    : Exception(message)
{
}
