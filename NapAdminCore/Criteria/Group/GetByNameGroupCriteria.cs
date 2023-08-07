namespace NapAdminCore.Criteria.Group;

using Fanex.Data.Repository;

public class GetByNameGroupCriteria : CriteriaBase
{
    public string GroupName { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Group_GetByName";
    }

    public override bool IsValid()
    {
        return true;
    }
}
