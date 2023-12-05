namespace NapAdminCore.Criteria.Site;

using Fanex.Data.Repository;

public class GetSiteUrlByIdCriteria : CriteriaBase
{
    public int SiteUrlId { get; set; }

    public override string GetSettingKey() => "NAP_Site_GetSiteURLById";

    public override bool IsValid() => true;
}
