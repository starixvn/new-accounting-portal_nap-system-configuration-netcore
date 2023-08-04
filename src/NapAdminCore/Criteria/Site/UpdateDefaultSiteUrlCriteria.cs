namespace NapAdminCore.Criteria.Site;

using Fanex.Data.Repository;

public class UpdateDefaultSiteUrlCriteria : CriteriaBase
{
    public int SiteUrlId { get; set; }

    public int SiteId { get; set; }

    public string Environment { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Site_UpdateDefaultSiteURL";
    }

    public override bool IsValid()
    {
        return true;
    }
}
