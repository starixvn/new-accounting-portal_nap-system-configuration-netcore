
namespace NapAdminCore.Criteria.User;

using Fanex.Data.Repository;

public class GetUserSiteLevelUserCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public int SiteId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_GetUserSiteLevel";
    }

    public override bool IsValid()
    {
        return true;
    }
}
