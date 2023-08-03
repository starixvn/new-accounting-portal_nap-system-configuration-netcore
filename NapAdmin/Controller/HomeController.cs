using System.Net.Sockets;
using System.Net;
using Fanex.NAP.Client.Helpers.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NapAdmin.Controller;

using Controller = Microsoft.AspNetCore.Mvc.Controller;

public class HomeController : Controller
{
    private readonly INapSession session;

    private readonly IConfiguration configuration;

    public HomeController(INapSession session, IConfiguration configuration)
    {
        this.session = session;
        this.configuration = configuration;
    }

    [Authorize]
    public IActionResult Index()
    {
        ViewBag.IsSessionAvailable = HttpContext.Session.IsAvailable;
        ViewBag.UserId = session.UserId;
        ViewBag.Username = session.UserName;
        var instanceName = Dns.GetHostName();
        var ip = Dns.GetHostEntry(instanceName).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
        ViewBag.InstanceName = instanceName;
        ViewBag.InstanceIp = ip;

        return View();
    }
}
