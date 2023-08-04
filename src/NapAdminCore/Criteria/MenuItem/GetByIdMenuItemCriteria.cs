namespace NapAdminCore.Criteria.MenuItem;

using Fanex.Data.Repository;

public class GetByIdMenuItemCriteria : CriteriaBase
{
    public int MenuItemId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_MenuItem_GetById";
    }

    public override bool IsValid()
    {
        return true;
    }
}
