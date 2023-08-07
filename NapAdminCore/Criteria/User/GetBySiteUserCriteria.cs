namespace NapAdminCore.Criteria.User;

using Fanex.Data.Repository;

public class GetBySiteUserCriteria : CriteriaBase
{
    public int SiteId { get; set; }

    public bool? IsActive { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_GetBySite";
    }

    public override bool IsValid()
    {
        return true;
    }
}
