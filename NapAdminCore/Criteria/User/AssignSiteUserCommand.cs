namespace NapAdminCore.Criteria.User;

using System;

public class AssignSiteUserCommand : UpdateCommandBase
{
    public int UserId { get; set; }

    public int SiteId { get; set; }

    public int Level { get; set; }

    public bool IsActive { get; set; }

    public bool IsNeverExpired { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public override string GetSettingKey() => "NAP_User_AssignNewSite";

    public override bool IsValid() => true;
}
