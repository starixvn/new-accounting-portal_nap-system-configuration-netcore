namespace NapAdminCore.NapApiCaching;

using Microsoft.Extensions.Configuration;

public interface ApiClientService
{
    Task<string> ReloadAllCache();

    Task<string> ReloadCache(JobCommand command);
}

public class ApiClientServiceImp(IConfiguration configuration) : ApiClientService, IDisposable
{
    private readonly HttpClient httpClient = CreateHttpClient(configuration);

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

    private static HttpClient CreateHttpClient(IConfiguration configuration)
    {
        var serviceUrl = configuration.GetSection("Fanex.NAP.Client:ServiceUrls").Value;
        var client = new HttpClient
        {
            BaseAddress = new Uri(serviceUrl)
        };

        return client;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        httpClient?.Dispose();
    }
}