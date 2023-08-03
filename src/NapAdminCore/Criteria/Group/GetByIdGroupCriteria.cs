
namespace NapAdminCore.Criteria.Group;

using Fanex.Data.Repository;

public class GetByIdGroupCriteria : CriteriaBase
{
    public int GroupId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Group_GetById";
    }

    public override bool IsValid()
    {
        return true;
    }
}
