using System.Net.Sockets;
using System.Net;
using Fanex.NAP.Client.Helpers.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NapAdmin.Controller;

using Fanex.NAP.Client.Configuration;
using NapAdminCore.Service;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

public class HomeController : BaseController
{
    private readonly INapSession napSession;

    public HomeController(INapSession napSession,
        INapSettings napSettings,
        IUserService userService)
    : base(napSession, napSettings, userService)
    {
        this.napSession = napSession;
    }

    [Authorize]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public IActionResult Test()
    {
        ViewBag.IsSessionAvailable = HttpContext.Session.IsAvailable;
        ViewBag.UserId = napSession.UserId;
        ViewBag.Username = napSession.UserName;
        var instanceName = Dns.GetHostName();
        var ip = Dns.GetHostEntry(instanceName).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
        ViewBag.InstanceName = instanceName;
        ViewBag.InstanceIp = ip;

        return View();
    }
}
