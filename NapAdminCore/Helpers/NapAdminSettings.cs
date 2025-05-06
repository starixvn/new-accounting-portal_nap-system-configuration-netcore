namespace NapAdminCore.Helpers;

using Microsoft.Extensions.Configuration;

public static class NapAdminSettings
{
    public static int OwpSiteId { get; private set; }

    public static string OwpApi { get; private set; }

    public static void LoadSettings(IConfiguration configuration)
    {
        OwpSiteId = configuration.GetValue<int>("OWP:SiteId");
        OwpApi = configuration.GetValue<string>("OWP:Api");
    }
}
