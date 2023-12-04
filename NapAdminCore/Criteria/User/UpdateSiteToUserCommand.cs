namespace NapAdminCore.Criteria.User;

using System;

public class UpdateSiteToUserCommand : UpdateCommandBase
{
    public int UserId { get; set; }

    public int SiteId { get; set; }

    public int Level { get; set; }

    public bool IsActive { get; set; }

    public DateTime ExpiredDate { get; set; }

    public bool IsNeverExpired { get; set; }

    public int ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public override string GetSettingKey() => "NAP_User_UpdateSite";

    public override bool IsValid() => true;
}
