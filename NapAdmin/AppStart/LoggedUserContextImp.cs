using Fanex.NAP.Client.Configuration;

namespace NapAdmin.AppStart;

using Fanex.NAP.Client.Helpers.Web;
using NapAdminCore.Criteria.Misc;
using NapAdminCore.Service;

public class LoggedUserContextImp: LoggedUserContext
{
    private readonly INapSettings napSettings;

    private readonly INapSession napSession;

    private readonly IHttpContextAccessor httpContextAccessor;

    public LoggedUserContextImp(INapSession napSession, INapSettings napSettings, IHttpContextAccessor httpContextAccessor)
    {
        this.napSession = napSession;
        this.napSettings = napSettings;
        this.httpContextAccessor = httpContextAccessor;
    }


    public LogExecSpCriteria GetLogExecSpCriteria()
    {
        return new LogExecSpCriteria
        {
            CurrentUserId = napSession.UserId,
            CurrentSiteId = napSettings.SiteId,
            LogDate = DateTime.Now,
            LoginIp = GetIp()
        };
    }

    public string GetIp()
    {
        var clientIP = httpContextAccessor.HttpContext.Request.Headers["x-forwarded-for"].ToString();
        if (!string.IsNullOrWhiteSpace(clientIP))
        {
            return clientIP;
        }

        return httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
    }
}
