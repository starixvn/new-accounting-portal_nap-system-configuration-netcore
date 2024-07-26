namespace NapAdminCore.NapApiCaching;

public class JobCommand(string command, int siteId)
{
    public const string Site = "SiteCommand";
    public const string Function = "FunctionCommand";
    public const string Group = "GroupCommand";
    public const string Menu = "MenuCommand";
    public const string UserFunctionPolicy = "UserFunctionPolicyCommand";
    public const string UserSiteLevel = "UserSiteLevelCommand";
    public const string User = "UserCommand";

    private static readonly Dictionary<string, int> priorityMap = new()
    {
        { Site, 0 },
        { Function, 1 },
        { Group, 2 },
        { Menu, 3 },
        { UserSiteLevel, 4 },
        { UserFunctionPolicy, 5 },
        { User, 6 }
    };

    public string Command { get; set; } = command;

    public int SiteId { get; set; } = siteId;

    public long Version { get; set; } = DateTimeOffset.UtcNow.Ticks;

    public int Priority => priorityMap.GetValueOrDefault(Command, int.MaxValue);
}
