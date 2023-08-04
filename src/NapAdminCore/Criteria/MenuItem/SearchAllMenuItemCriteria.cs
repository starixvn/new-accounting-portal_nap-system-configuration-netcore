namespace NapAdminCore.Criteria.MenuItem;

using Fanex.Data.Repository;

public class SearchAllMenuItemCriteria : CriteriaBase
{
    public string Environment { get; set; }

    public string MenuItemName { get; set; }

    public bool? IsActive { get; set; }

    public int SiteId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_MenuItem_Search";
    }

    public override bool IsValid()
    {
        return true;
    }
}
