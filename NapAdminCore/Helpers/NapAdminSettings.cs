namespace NapAdminCore.Helpers;

using Microsoft.Extensions.Configuration;

public static class NapAdminSettings
{
    public static int OWPSiteID { get; private set; }

    public static void LoadSettings(IConfiguration configuration)
    {
        OWPSiteID = configuration.GetValue<int>("AppConfig:OWPSiteID");
    }
}
