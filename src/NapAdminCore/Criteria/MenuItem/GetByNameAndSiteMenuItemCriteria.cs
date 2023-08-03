
namespace NapAdminCore.Criteria.MenuItem;

using Fanex.Data.Repository;

public class GetByNameAndSiteMenuItemCriteria : CriteriaBase
{
    public string MenuItemName { get; set; }

    public int SiteId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_MenuItem_GetByNameAndSite";
    }

    public override bool IsValid()
    {
        return true;
    }
}
