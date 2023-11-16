namespace NapAdminCore.Criteria.Trace;

using System;
using Fanex.Data.Repository;

public class InsertLogUserLoginCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public int SiteId { get; set; }

    public string IP { get; set; }

    public DateTime LogDate { get; set; }

    public string Doing { get; set; }

    public string DomainName { get; set; }

    public string BrowserName { get; set; }

    public string Platform { get; set; }

    public string CountryCode { get; set; }

    public int Duration { get; set; }

    public override bool IsValid() => true;

    public override string GetSettingKey() => "NAP_LogUserLogin_Insert";
}
