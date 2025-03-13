using Fanex.Data;
using Fanex.NAP.Client.Attributes;
using Microsoft.AspNetCore.Mvc;
using NapAdmin.AppStart;
using NapAdminCore.Helpers;
using Fanex.Caching.Redis;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.NAP.Client;
using ONEbook.UM.DependencyInjection;
using ONEbook.UM.Filters;
Environment.SetEnvironmentVariable("DOTNET_hostBuilder:reloadConfigOnChange", "true");
var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddExceptionHandler(configuration);
services.AddFanexLogSentry(configuration);
services.AddRedisCache(configuration.GetSection("Fanex.Caching.Redis"));
services.AddFanexData(configuration);
services.AddNapClient(configuration);
services.AddNapAdminCore();
services.AddNapSession(configuration);
services.AddNapAuthorization();
services.AddNapAuthentication(configuration);
services.AddOneBookUmForWeb(configuration);
services.AddNapApiCaching();
services.AddRevokePermissionJob();

services.AddRazorPages();
services.AddControllersWithViews(options =>
{
    options.Filters.Add(new ServiceFilterAttribute(typeof(CheckUMFilterAttribute)));
    options.Filters.Add(new SessionAuthorizeAttribute());
});

services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseCookiePolicy();
app.UseSession();
app.UseMiddleware<ErrorHandler>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

NapAdminSettings.LoadSettings(configuration);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
