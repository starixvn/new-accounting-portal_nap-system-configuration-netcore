namespace NapAdminCore.NapApiCaching;

public interface ApiClientService
{
    Task<string> ReloadAllCache();

    Task<string> ReloadCache(JobCommand command);

    Task<Dictionary<string, string>> GetAllCache();
}

public class ApiClientServiceImp(HttpClient httpClient) : ApiClientService
{
    public async Task<string> ReloadAllCache()
    {
        using var response = await httpClient.GetAsync("cache/reload");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> ReloadCache(JobCommand command)
    {
        using var response = await httpClient.GetAsync($"cache?command={command.Command}&siteId={command.SiteId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }


    public async Task<Dictionary<string, string>> GetAllCache()
    {
        string[] keys = ["SiteUserFunctionPoliciesCache", 
            "SiteUserMenuItemsCache",
            "FunctionsCache",
            "GroupsCache",
            "UserSiteLevelsByUserCache",
            "UserSiteLevelsBySiteCache",
            "SiteMenuItemsCache",
            "UserOtpCache",
            "UsersCache"
        ];

        var result = new Dictionary<string, string>();
        foreach (var key in keys)
        {
            var data = await GetData(key);
            result.Add(key, data);
        }

        return result;
    }

    private async Task<string> GetData(string key)
    {
        using var response = await httpClient.GetAsync($"cache/{key}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}