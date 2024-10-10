namespace MyBestJob.BLL.Exceptions;

public class UserNotSignedInException(string message)
    : Exception(message)
{
}
