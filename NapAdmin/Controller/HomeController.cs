using System.Net.Sockets;
using System.Net;
using Fanex.NAP.Client.Helpers.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Fanex.Data;
using Fanex.Logging;
using System.Diagnostics;
using System.Runtime;
using System.Text;
using Fanex.NAP.Client.Configuration;
using NapAdminCore.Service;
using ONEbook.UM.AspNetCore.Filters;

namespace NapAdmin.Controller;

public class HomeController(INapSession napSession,
        INapSettings napSettings,
        IUserService userService)
    : BaseController(napSession, napSettings, userService)
{
    [Authorize]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public IActionResult Test()
    {
        ViewBag.IsSessionAvailable = HttpContext.Session.IsAvailable;
        ViewBag.UserId = NapSession.UserId;
        ViewBag.Username = NapSession.UserName;
        var instanceName = Dns.GetHostName();
        var ip = Dns.GetHostEntry(instanceName).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
        ViewBag.InstanceName = instanceName;
        ViewBag.InstanceIp = ip;

        Logger.Log.Info("Test Log");

        return View();
    }

    [AllowAnonymous]
    [HttpGet]
    [SkipUMFilter]
    public IActionResult Gc()
    {
        GC.Collect(2, GCCollectionMode.Aggressive | GCCollectionMode.Forced, true, true);
        const int megabyte = 1024 * 1024;
        var stringBuilder = new StringBuilder();
        var sw = Stopwatch.StartNew();
        sw.Start();

        using var currentProcess = Process.GetCurrentProcess();
        currentProcess.Refresh();
        var memory = currentProcess.WorkingSet64 / megabyte;

        stringBuilder.AppendFormat("Total memory - Before GC.Collect: {0} Mb", memory);

        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive | GCCollectionMode.Forced, true, true);
        GC.WaitForPendingFinalizers();
        GC.WaitForFullGCComplete();

        currentProcess.Refresh();
        memory = currentProcess.WorkingSet64 / megabyte;

        var duration = sw.ElapsedMilliseconds;
        stringBuilder.AppendFormat("GC.Collected. Usage memory = {0}. Duration: {1} milliseconds", memory, duration);

        return Ok(stringBuilder.ToString());
    }

    [AllowAnonymous]
    [HttpGet]
    [SkipUMFilter]
    public async Task<IActionResult> ClearCache(int count = 10)
    {
        var result = await CacheManager.ClearCache(count);

        return Ok(result);
    }
}
