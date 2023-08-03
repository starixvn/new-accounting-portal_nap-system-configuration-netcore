
namespace NapAdminCore.Criteria.User;

using Fanex.Data.Repository;

public class ChangeUserSiteStatusUserCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public int SiteId { get; set; }

    public bool IsActive { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_ChangeUserSiteStatus";
    }

    public override bool IsValid()
    {
        return true;
    }
}
