namespace MyBestJob.DAL.Constants;

public static partial class Constants
{
    public static class Google
    {
        public static readonly string GrantType = "authorization_code";

        public static readonly string AccessType = "offline";
        public static readonly string ResponseType = "code";
        public static readonly string Prompt = "consent";
        public static readonly string State = "/";
    }
}
