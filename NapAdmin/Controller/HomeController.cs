using Fanex.NAP.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Fanex.Data;
using Fanex.NAP.Client.Configuration;
using NapAdminCore.Service;
using ONEbook.UM.AspNetCore.Filters;

namespace NapAdmin.Controller;

public class HomeController(
    INapSession napSession,
    INapSettings napSettings,
    IUserService userService)
    : BaseController(napSession, napSettings, userService)
{
    [Authorize]
    public IActionResult Index()
    {
        return View();
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
