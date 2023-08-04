using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Web;
using Fanex.NAP.Client;
using Fanex.NAP.Signin.AspNetCore.Attribute;
using Microsoft.AspNetCore.Mvc;
using NapAdmin.AppStart;
using ONEbook.UM.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddFanex(configuration);
services.AddNapServices(configuration);
services.AddNapAdminCore();
services.AddNapSession(configuration);
services.AddNapAuthorization();
services.AddNapAuthentication(configuration);
services.AddMaintenance(configuration);
services.AddCompression();

services.AddRazorPages();
services.AddControllersWithViews(options =>
{
    options.Filters.Add(new ServiceFilterAttribute(typeof(CheckUMFilterAttribute)));
    options.Filters.Add(new SessionAuthorizeAttribute());
});

services.AddControllersWithViews();

var app = builder.Build();

app.UseResponseCompression();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseSession();
app.UseMiddleware<ErrorHandler>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

NapClientManager
    .UseConfig(app.Services.GetRequiredService<INapSettings>())
    .UseSession(app.Services.GetRequiredService<INapSession>());


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
