
namespace NapAdminCore.Business;

public static class Constant
{
    public const string Space = " ";

    public const string Ok = "OK";

    public const string Error = "ERROR";

    public const string Uat = "UAT";

    public const string Live = "LIVE";

    public const string SiteId = "SiteId";

    public const string Environment = "Environment";

    public const int SearchAllSite = -9999;

    public const string IT = "IT";

    public enum Policy
    {
        None = 0,

        Read = 1,

        Host = 2
    }

    public enum Crud
    {
        Read = 0,

        Host = 1
    }

    public enum Level
    {
        User = 0,

        SiteAdmin = 50,

        PortalAdmin = 100
    }

    public enum ReCheckInResult
    {
        Ok = 0,

        DownTime = 1,

        KickedOut = 2
    }
}
