
namespace NapAdminCore.Criteria.User;

using System;
using Fanex.Data.Repository;

public class UpdateSiteToUserCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public int SiteId { get; set; }

    public int Level { get; set; }

    public bool IsActive { get; set; }

    public int ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_UpdateSite";
    }

    public override bool IsValid()
    {
        return true;
    }
}
