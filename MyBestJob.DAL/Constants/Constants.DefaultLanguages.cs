namespace MyBestJob.DAL.Constants;

public static partial class Constants
{
    public static class DefaultLanguages
    {
        public const string AcceptLanguageHeader = "Accept-Language";
        public const string DefaultLanguage = "en-US";

        public static readonly Guid HungaryId =
            Guid.Parse("E5531E9C-FC9E-470E-9792-10BCFDFAB45A");
        public static readonly Guid EnglishId =
            Guid.Parse("4A52C5A7-D6E1-4BC0-972D-E6AB5E4536CC");
    }
}
