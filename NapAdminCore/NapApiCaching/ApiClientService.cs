namespace NapAdminCore.NapApiCaching;

using Microsoft.Extensions.Configuration;

public interface ApiClientService
{
    Task<string> ReloadAllCache();

    Task<string> ReloadCache(JobCommand command);
}

public class ApiClientServiceImp : ApiClientService
{
    private readonly HttpClient httpClient;

    public ApiClientServiceImp(HttpClient httpClient, IConfiguration configuration)
    {
        var serviceUrl = configuration.GetSection("Fanex.NAP.Client:ServiceUrls").Value;
        this.httpClient = httpClient;
        this.httpClient.BaseAddress = new Uri(serviceUrl);
    }
    
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
}