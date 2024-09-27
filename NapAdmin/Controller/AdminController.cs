namespace NapAdmin.Controller;

using System.Text;
using Fanex.Caching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using NapAdminCore.NapApiCaching;
using Newtonsoft.Json;
using StackExchange.Redis;

public class AdminController(ICache cache, ApiClientService apiClientService, IConfiguration configuration) : Controller
{
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Cache()
    {
        var options = ReadConfigurationOptions();
        var redis = ConnectionMultiplexer.Connect(options);
        var endPoints = redis.GetEndPoints();
        var appName = configuration.GetSection("Fanex.Caching.Redis:ApplicationName").Value;
        var keyPrefix = $"{{{appName}}}:{{";
        var database = options.DefaultDatabase ?? 1;

        var keys = endPoints.Select(e => redis.GetServer(e).Keys(database, $"{{{appName}}}:*"))
            .SelectMany(k => k)
            .Select(k => k.ToString().Replace(keyPrefix, string.Empty))
            .Select(k => k.Remove(k.Length - 1))
            .Distinct()
            .OrderBy(k => k)
            .ToArray();

        ViewBag.CacheKeys = keys;
        return View("Cache");
    }

    private ConfigurationOptions ReadConfigurationOptions()
    {
        var redisConfigSection = configuration.GetSection("Fanex.Caching.Redis:ConnectionOptions");
        var endPoints = redisConfigSection.GetSection("EndPoints").Get<string[]>();
        var password = redisConfigSection["Password"];
        var defaultDatabase = int.Parse(redisConfigSection["DefaultDatabase"]);

        var configOptions = new ConfigurationOptions
        {
            Password = password,
            DefaultDatabase = defaultDatabase,
            EndPoints = new EndPointCollection()
        };

        foreach (var endPoint in endPoints)
        {
            configOptions.EndPoints.Add(endPoint);
        }

        return configOptions;
    }

    public ActionResult DeleteCache(string cacheKey)
    {
        cache.Remove(cacheKey);

        return Redirect("Cache");
    }

    public async Task<IActionResult> CacheContent(string cacheKey, string typeName = null)
    {
        var (_, value) = await cache.TryGetAsync<string>(cacheKey);

        ViewBag.CacheContent = value;

        return View("CacheContent");
    }

    [AllowAnonymous]
    public async Task<IActionResult> ReloadNapApiCache()
    {
        await apiClientService.ReloadAllCache();
        return Redirect("Cache");
    }

    [AllowAnonymous]
    public async Task<IActionResult> GetAllCache()
    {
        var data = await apiClientService.GetAllCache();
        var stringBuilder = new StringBuilder();
        foreach (var key in data.Keys)
        {
            stringBuilder.AppendLine(key);
            stringBuilder.AppendLine(data[key]);
            stringBuilder.AppendLine();
        }

        return Content(stringBuilder.ToString());
    }

    [AllowAnonymous]
    public async Task<IActionResult> TestCache(string key = "MyKey")
    {
        var cacheOptions = new CacheItemOptions().SetAbsoluteExpiration(DateTimeOffset.Now.AddDays(1));
        await cache.SetAsync(key, DateTimeOffset.Now.ToString(), cacheOptions);

        var (_, value) = await cache.TryGetAsync<string>(key);

        return Content(value);
    }
}