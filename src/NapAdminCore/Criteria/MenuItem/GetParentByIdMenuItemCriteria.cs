namespace NapAdminCore.Criteria.MenuItem;

using Fanex.Data.Repository;

public class GetParentByIdMenuItemCriteria : CriteriaBase
{
    public int MenuItemId { get; set; }

    public string Environment { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_MenuItem_GetParentById";
    }

    public override bool IsValid()
    {
        return true;
    }
}
