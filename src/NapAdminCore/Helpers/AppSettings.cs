namespace NapAdminCore.Helpers;

public static class AppSettings
{
    public static int OWPSiteID { get; private set; }

    public static void LoadSettings(IConfiguration configuration)
    {
        OWPSiteID = configuration.GetValue<int>("AppConfig:OWPSiteID");
    }
}
