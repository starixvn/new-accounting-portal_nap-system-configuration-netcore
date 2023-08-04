namespace NapAdminCore.Criteria.MenuItem;

using Fanex.Data.Repository;

public class GetAllMenuItemCriteria : CriteriaBase
{
    public string Environment { get; set; }

    public bool? IsActive { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_MenuItem_GetAll";
    }

    public override bool IsValid()
    {
        return true;
    }
}
