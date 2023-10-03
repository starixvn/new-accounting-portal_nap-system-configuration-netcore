namespace NapAdminCore.Criteria.Group;

using Fanex.Data.Repository;

public class SearchGroupExcludeFunctionCriteria : CriteriaBase
{
    public string GroupName { get; set; }

    public int SiteId { get; set; }

    public int FunctionId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Group_SearchExcludeFunction";
    }

    public override bool IsValid()
    {
        return true;
    }
}
