namespace MyBestJob.BLL.Exceptions;

public class HttpErrorException(string message)
    : Exception(message)
{
}
