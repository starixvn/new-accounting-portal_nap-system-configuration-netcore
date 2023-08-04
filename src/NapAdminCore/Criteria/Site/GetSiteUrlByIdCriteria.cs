namespace NapAdminCore.Criteria.Site;

using Fanex.Data.Repository;

public class GetSiteUrlByIdCriteria : CriteriaBase
{
    public int SiteUrlId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Site_GetSiteURLById";
    }

    public override bool IsValid()
    {
        return true;
    }
}
