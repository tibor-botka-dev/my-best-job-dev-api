namespace MyBestJob.BLL.Exceptions;

public class CanNotChangePasswordOfExternalAccountException(string email)
    : Exception($"Can not change password with email: '{email}'. Account is external.")
{
    public string Email { get; set; } = email;
}
