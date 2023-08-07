namespace NapAdminCore.Criteria.User;

using Fanex.Data.Repository;

public class GetByGroupAndSiteUserCriteria : CriteriaBase
{
    public int GroupId { get; set; }

    public int SiteId { get; set; }

    public bool? IsActive { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_GetByGroupAndSite";
    }

    public override bool IsValid()
    {
        return true;
    }
}
