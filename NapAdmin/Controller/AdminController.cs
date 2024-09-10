namespace NapAdmin.Controller;

using Fanex.Caching;
using Microsoft.AspNetCore.Authorization;
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

    public ActionResult CacheContent(string cacheKey, string typeName = null)
    {
        string cacheContent;
        if (string.IsNullOrEmpty(typeName))
        {
            cache.TryGet(cacheKey, out cacheContent);
        }
        else
        {
            var type = Type.GetType(typeName);
            var method = typeof(ICache).GetMethod(nameof(ICache.TryGet));
            var generic = method.MakeGenericMethod(type);
            var parameters = new object[] { cacheKey, null };
            generic.Invoke(cache, parameters);

            cacheContent = JsonConvert.SerializeObject(parameters[1]);
        }

        ViewBag.CacheContent = cacheContent;
        return View("CacheContent");
    }

    [AllowAnonymous]
    public async Task<IActionResult> ReloadNapApiCache()
    {
        await apiClientService.ReloadAllCache();
        return Redirect("Cache");
    }
}