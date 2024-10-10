namespace MyBestJob.DAL.Constants;

public static partial class Constants
{
    public static class DefaultRoles
    {
        public static readonly Guid AdministratorRoleId =
            Guid.Parse("F71B388F-00BE-4ACF-AB68-8AD20D164206");
        public static readonly Guid UserRoleId =
            Guid.Parse("68F9E4BA-3912-4232-B73E-B43FBA3BEB0A");
        public static readonly Guid WebsiteSupporterRoleId =
            Guid.Parse("811F53E5-1A29-43D0-A5E2-120BEAD3D417");
        public static readonly Guid SpectatorRoleId =
            Guid.Parse("4F940196-1429-4134-A957-6A3978A553F0");
    }
}
