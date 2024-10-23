namespace MyBestJob.DAL.Constants;

public static partial class Constants
{
    public static class ApiRoutes
    {
        private const string Api = "MyBestJob.API.";

        public const string Init = Api + nameof(Init);

        public const string SignIn = Api + nameof(SignIn);
        public const string SignUp = Api + nameof(SignUp);
        public const string SignOut = Api + nameof(SignOut);
        public const string RefreshToken = Api + nameof(RefreshToken);
        public const string ConfirmAccount = Api + nameof(ConfirmAccount);
        public const string ForgotPassword = Api + nameof(ForgotPassword);
        public const string ResetPassword = Api + nameof(ResetPassword);
        public const string ChangePassword = Api + nameof(ChangePassword);

        public const string User = Api + nameof(User);
        public const string GetUser = Api + nameof(GetUser);
        public const string GetUserAvatar = Api + nameof(GetUserAvatar);

        public const string Setting = Api + nameof(Setting);
        public const string GetMailSetting = Api + nameof(GetMailSetting);
        public const string GetEmailTemplates = Api + nameof(GetEmailTemplates);
        public const string UpdateMailSetting = Api + nameof(UpdateMailSetting);
        public const string UpdateEmailTemplate = Api + nameof(UpdateEmailTemplate);
        public const string CreateEmailTemplateValue = Api + nameof(CreateEmailTemplateValue);
        public const string UpdateEmailTemplateValue = Api + nameof(UpdateEmailTemplateValue);
        public const string DeleteEmailTemplateValue = Api + nameof(DeleteEmailTemplateValue);

        public const string GoogleSignUp = Api + nameof(GoogleSignUp);
        public const string GoogleSignIn = Api + nameof(GoogleSignIn);

    }
}
