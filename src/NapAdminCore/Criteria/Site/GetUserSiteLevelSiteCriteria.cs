
namespace NapAdminCore.Criteria.Site;

using Fanex.Data.Repository;

public class GetUserSiteLevelSiteCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public int SiteId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Site_GetUserSiteLevel";
    }

    public override bool IsValid()
    {
        return true;
    }
}
