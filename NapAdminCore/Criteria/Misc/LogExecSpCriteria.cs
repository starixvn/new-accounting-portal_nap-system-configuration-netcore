namespace NapAdminCore.Criteria.Misc;

using System;

public class LogExecSpCriteria
{
    public int CurrentUserId { get; set; }

    public int CurrentSiteId { get; set; }

    public DateTime LogDate { get; set; }

    public string LoginIp { get; set; }
}
