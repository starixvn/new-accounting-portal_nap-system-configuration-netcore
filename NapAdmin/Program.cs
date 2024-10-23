using Fanex.Data;
using Fanex.NAP.Client.Attributes;
using Microsoft.AspNetCore.Mvc;
using NapAdmin.AppStart;
using ONEbook.UM.AspNetCore.Filters;
using NapAdminCore.Helpers;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddFanexLog(configuration);
services.AddFanexCaching(configuration);
services.AddFanexData(configuration);
services.AddNapClient(configuration);
services.AddNapAdminCore();
services.AddNapSession(configuration);
services.AddNapAuthorization();
services.AddNapAuthentication(configuration);
services.AddOneBookUm(configuration);
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
