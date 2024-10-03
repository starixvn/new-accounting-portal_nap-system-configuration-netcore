namespace NapAdmin.AppStart;

using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.AspNetCore;
using Fanex.Logging.Sentry.RavenClient;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Authorization;
using NapAdminCore.NapApiCaching;
using NapAdminCore.Repository;
using NapAdminCore.Service;
using SharpRaven;
using StackExchange.Redis;
using GroupService = NapAdminCore.Service.GroupService;
using IGroupService = NapAdminCore.Service.IGroupService;
using ISiteService = NapAdminCore.Service.ISiteService;
using IUserService = NapAdminCore.Service.IUserService;
using SiteService = NapAdminCore.Service.SiteService;
using UserService = NapAdminCore.Service.UserService;

public static class IServiceCollectionExtensions
{
    public static void AddFanexLog(this IServiceCollection services, IConfiguration configuration)
    {
        LogManager
            .SetDefaultLogCategory(configuration.GetSection("LogCategory").Value)
            .Use(new List<ILogEngine>
            {
                new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(configuration.GetSection("SentryEngineOptions:Dsn").Value)
                })
            });

        ExceptionHandler
            .UseConfig(new LoggingConfiguration());

        services.AddSingleton<ILogger, Logger>();
    }

    public static void AddNapAdminCore(this IServiceCollection services)
    {
        services.AddScoped<IDynamicRepository, DynamicRepository>();
        services.AddScoped<UpdateRepository, UpdateRepositoryImp>();
        services.AddScoped<IFunctionService, FunctionService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IMenuItemService, MenuItemService>();
        services.AddScoped<ISiteService, SiteService>();
        services.AddScoped<ITraceService, TraceService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<LoggedUserContext, LoggedUserContextImp>();
    }

    public static void AddFanexCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICacheService, CacheService>();
        services.AddRedisCache(configuration.GetSection("Fanex.Caching.Redis"));
    }

    public static void AddNapSession(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSession(options =>
        {
            options.Cookie.IsEssential = true;
            options.IdleTimeout = TimeSpan.FromMinutes(int.Parse(configuration.GetSection("AuthSettings:SessionTimeOut").Value));
            options.Cookie.Name = "Nap-Admin.Session";
        });

        var redisConfig = configuration.GetSection("AuthSettings:RedisConfig").Value;
        var redisMultiplexer = ConnectionMultiplexer.Connect(redisConfig);

        services.AddDataProtection()
            .PersistKeysToStackExchangeRedis(redisMultiplexer, "Nap-Admin-ProtectionKeys");

        services.AddSingleton(redisMultiplexer);

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfig;
            options.InstanceName = "Nap-Admin.Session-";
        });

    }
    
    public static void AddNapAuthorization(this IServiceCollection services)
    {
        services.AddControllers(config =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            config.Filters.Add(new AuthorizeFilter(policy));
        });
    }

    public static void AddNapAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = configuration.GetSection("AuthSettings:LoginPath").Value;
                options.LogoutPath = configuration.GetSection("AuthSettings:LogoutPath").Value;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(configuration.GetSection("AuthSettings:CookieTimeOut").Value));
                options.Cookie = new CookieBuilder
                {
                    Name = "Nap-Admin.Cookie",
                    SecurePolicy = CookieSecurePolicy.SameAsRequest
                };
                options.Events.OnSigningOut = context =>
                {
                    context.Response.Redirect(options.LoginPath);
                    return Task.CompletedTask;
                };
            });
    }

    public static void AddNapApiCaching(this IServiceCollection services)
    {
        services.AddHttpClient<ApiClientServiceImp>();
        services.AddSingleton<ApiClientService, ApiClientServiceImp>();
        services.AddSingleton<DataCachingRealTimeJob>();

        services.AddHostedService<DataCachingRealTimeJob>();
    }

    public static void AddRevokePermissionJob(this IServiceCollection services)
    {
        services.AddSingleton<RevokePermissionJob>();

        services.AddHostedService<RevokePermissionJob>();
    }
}
