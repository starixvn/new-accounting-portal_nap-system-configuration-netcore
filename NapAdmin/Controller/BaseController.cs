namespace NapAdmin.Controller;

using System.Text;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NapAdminCore.Service;
using NapAdminCore.Web;
using AdminConstant = NapAdminCore.Models.AdminConstant;

public class BaseController(INapSession napSession, INapSettings napSettings, IUserService userService)
    : Controller
{
    protected const string EmptyString = "";

    protected const int NapAdminId = 1;

    protected IUserService userService = userService;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        ViewFlags = new ViewFlags
        {
            IsLive = Environment.Equals(AdminConstant.Live),
            IsNapAdminSite = napSettings.SiteId.Equals(NapAdminId),
            HasEditRight = false,
            IsReCheckin = napSettings.EnableReCheckIn
        };
        ViewBag.NapSession = NapSession;
        ViewBag.ViewFlags = ViewFlags;
    }

    protected ViewFlags ViewFlags
    {
        get => ViewBag.ViewFlags;

        set => ViewBag.ViewFlags = value;
    }

    protected int SiteId => napSettings.SiteId;

    protected string Environment => napSettings.Environment;

    protected bool UserIsPortalAdmin => NapSession.Level == (int)AdminConstant.Level.PortalAdmin;

    protected bool UserIsSiteAdmin => NapSession.Level == (int)AdminConstant.Level.SiteAdmin;

    protected INapSession NapSession { get; } = napSession;

    public bool HasRight(string functionKey, AdminConstant.Policy policy)
    {
        var functionPolicy = userService.GetUserSiteFunctionKeyPolicy(NapSession.UserId, napSettings.SiteId, functionKey);

        return functionPolicy >= (int)policy;
    }

    public bool HasRightOnSite(int siteId, string functionKey, AdminConstant.Policy policy)
    {
        var functionPolicy = userService.GetUserSiteFunctionKeyPolicy(NapSession.UserId, siteId, functionKey);

        return functionPolicy >= (int)policy;
    }

    protected static string ConvertToKey(string name)
    {
        var functionKey = name.Replace(AdminConstant.Space, string.Empty);

        var sb = new StringBuilder();
        var length = functionKey.Length;

        for (var i = 0; i < length; i++)
        {
            var item = functionKey[i];

            if (char.IsLetterOrDigit(item))
            {
                sb.Append(item);
            }
            else
            {
                sb.Append("_");
            }
        }

        return sb.ToString();
    }
}
