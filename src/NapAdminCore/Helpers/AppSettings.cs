namespace NapAdminCore.Helpers;

public static class AppSettings
{
    public static string EmailSenderName { get; private set; }

    public static void LoadSettings(IConfiguration configuration)
    {
        EmailSenderName = configuration.GetValue<string>("AppConfig:EmailSenderName");
    }
}
