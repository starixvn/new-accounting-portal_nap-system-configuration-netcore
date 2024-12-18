using Fanex.NAP.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Fanex.Data;
using Fanex.NAP.Client.Configuration;
using NapAdminCore.Service;

namespace NapAdmin.Controller;

using ONEbook.UM.Filters;

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
    public async Task<IActionResult> ClearCache()
    {
        var result = await CacheManager.ClearCacheAsync();

        return Ok(result);
    }
}
