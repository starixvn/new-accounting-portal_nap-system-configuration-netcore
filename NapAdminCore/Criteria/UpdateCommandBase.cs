namespace NapAdminCore.Criteria;

using Fanex.Data.Repository;
using Misc;

public abstract class UpdateCommandBase: NonQueryCommand
{
    public int CurrentUserId { get; set; }

    public int CurrentSiteId { get; set; }

    public DateTime LogDate { get; set; }

    public string LoginIp { get; set; }

    public int ErrCode { get; set; } = 0;

    public string ErrMsg { get; set; } = string.Empty;

    public void CopyFromLogCriteria(LogExecSpCriteria logCriteria)
    {
        CurrentUserId = logCriteria.CurrentUserId;
        CurrentSiteId = logCriteria.CurrentSiteId;
        LogDate = logCriteria.LogDate;
        LoginIp = logCriteria.LoginIp;
    }
}