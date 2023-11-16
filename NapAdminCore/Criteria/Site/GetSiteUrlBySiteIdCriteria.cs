namespace NapAdminCore.Criteria.Site;

using Fanex.Data.Repository;

public class GetSiteUrlBySiteIdCriteria : CriteriaBase
{
    public int SiteId { get; set; }

    public string Environment { get; set; }

    public override string GetSettingKey() => "NAP_Site_GetSiteURLBySiteId";

    public override bool IsValid() => true;
}
