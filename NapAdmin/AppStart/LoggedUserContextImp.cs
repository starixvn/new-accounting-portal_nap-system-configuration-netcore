using Fanex.NAP.Client.Configuration;

namespace NapAdmin.AppStart;

using Fanex.NAP.Client;
using NapAdminCore.Criteria.Misc;
using NapAdminCore.Service;

public class LoggedUserContextImp(INapSession napSession, INapSettings napSettings,
        IHttpContextAccessor httpContextAccessor)
    : LoggedUserContext
{
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
        return !string.IsNullOrWhiteSpace(clientIP) ? clientIP : httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
    }
}
