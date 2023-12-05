namespace NapAdmin.AppStart;

using Fanex.Caching;
using Fanex.Data.Repository;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.AspNetCore;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Web;
using Fanex.NAP.Client.Services;
using Fanex.NAP.Client.Session;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Authorization;
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

using GroupServiceClient = Fanex.NAP.Client.Services.GroupService;
using IGroupServiceClient = Fanex.NAP.Client.Services.IGroupService;
using ISiteServiceClient = Fanex.NAP.Client.Services.ISiteService;
using IUserServiceClient = Fanex.NAP.Client.Services.IUserService;
using SiteServiceClient = Fanex.NAP.Client.Services.SiteService;
using UserServiceClient = Fanex.NAP.Client.Services.UserService;

public static class IServiceCollectionExtensions
{
    public static NapSettings NapSettings { get; private set; }

    public static void AddFanex(this IServiceCollection services, IConfiguration configuration)
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
    }

    public static void AddNapServices(this IServiceCollection services, IConfiguration configuration)
    {
        var napSettings = new NapSettings(configuration);
        NapSettings = napSettings;

        services.AddSingleton<INapSettings>(napSettings);
        services.AddSingleton<ILogger, Logger>();
        services.AddSingleton<INapSession, NapSession>();
        services.AddSingleton<ICacheService, CacheService>();

        services.AddSingleton<IGroupServiceClient, GroupServiceClient>();
        services.AddSingleton<IUserServiceClient, UserServiceClient>();
        services.AddSingleton<ISiteServiceClient, SiteServiceClient>();
        services.AddSingleton<ISecurityService, SecurityService>();
    }

    public static void AddNapAdminCore(this IServiceCollection services)
    {
        services.AddScoped<IDynamicRepository, DynamicRepository>();
        services.AddScoped<ISimpleRepository, SimpleSimpleRepository>();
        services.AddScoped<IFunctionService, FunctionService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IMenuItemService, MenuItemService>();
        services.AddScoped<ISiteService, SiteService>();
        services.AddScoped<ITraceService, TraceService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<LoggedUserContext, LoggedUserContextImp>();
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
            .PersistKeysToStackExchangeRedis(redisMultiplexer, "Fanex.Settlement-ProtectionKeys");

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
                    Name = "Fanex.Settlement.Cookie",
                    SecurePolicy = CookieSecurePolicy.SameAsRequest
                };
                options.Events.OnSigningOut = context =>
                {
                    context.Response.Redirect(options.LoginPath);
                    return Task.CompletedTask;
                };
            });
    }
    
}
