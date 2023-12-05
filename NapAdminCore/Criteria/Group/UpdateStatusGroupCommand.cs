namespace NapAdminCore.Criteria.Group;

using System;

public class UpdateStatusGroupCommand : UpdateCommandBase
{
    public int GroupId { get; set; }

    public bool IsActive { get; set; }

    public int ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public override string GetSettingKey() => "NAP_Group_UpdateStatus";

    public override bool IsValid() => true;
}
