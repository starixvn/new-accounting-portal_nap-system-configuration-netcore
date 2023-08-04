namespace NapAdminCore.Criteria.MenuItem;

using System;
using Fanex.Data.Repository;

public class UpdateStatusMenuItemCriteria : CriteriaBase
{
    public int MenuItemId { get; set; }

    public bool IsActive { get; set; }

    public int ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_MenuItem_UpdateStatus";
    }

    public override bool IsValid()
    {
        return true;
    }
}
