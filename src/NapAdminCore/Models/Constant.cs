namespace NapAdminCore.Models;

public static class Constant
{
    public const string UAT = "UAT";

    public const string LIVE = "LIVE";

    public const string Environment = "Environment";

    public enum Policy
    {
        None = 0,

        Read = 1,

        Host = 2
    }

    public enum Level
    {
        User = 0,

        SiteAdmin = 50,

        PortalAdmin = 100
    }
}
