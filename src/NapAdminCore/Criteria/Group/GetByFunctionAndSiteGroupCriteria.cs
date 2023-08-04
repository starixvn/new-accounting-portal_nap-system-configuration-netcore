namespace NapAdminCore.Criteria.Group;

using Fanex.Data.Repository;

public class GetByFunctionAndSiteGroupCriteria : CriteriaBase
{
    public int FunctionId { get; set; }

    public int SiteId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Group_GetGroupByFunctionAndSite";
    }

    public override bool IsValid()
    {
        return true;
    }
}
