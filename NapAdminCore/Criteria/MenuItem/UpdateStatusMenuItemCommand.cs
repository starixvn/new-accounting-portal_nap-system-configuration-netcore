namespace NapAdminCore.Criteria.MenuItem;

using System;

public class UpdateStatusMenuItemCommand : UpdateCommandBase
{
    public int MenuItemId { get; set; }

    public bool IsActive { get; set; }

    public int ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public override string GetSettingKey() => "NAP_MenuItem_UpdateStatus";

    public override bool IsValid() => true;
}
