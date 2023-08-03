
namespace NapAdminCore.Criteria.Misc;

using System;
using Fanex.Data.Repository;

public class LogExecSpCriteria : CriteriaBase
{
    public int CurrentUserId { get; set; }

    public int CurrentSiteId { get; set; }

    public DateTime LogDate { get; set; }

    public string LoginIp { get; set; }

    public override string GetSettingKey() => string.Empty;

    public override bool IsValid()
    {
        return true;
    }
}
