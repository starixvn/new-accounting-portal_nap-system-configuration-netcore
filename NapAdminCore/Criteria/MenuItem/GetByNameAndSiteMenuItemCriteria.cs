namespace NapAdminCore.Criteria.MenuItem;

using Fanex.Data.Repository;

public class GetByNameAndSiteMenuItemCriteria : CriteriaBase
{
    public string MenuItemName { get; set; }

    public int SiteId { get; set; }

    public override string GetSettingKey() => "NAP_MenuItem_GetByNameAndSite";

    public override bool IsValid() => true;
}
