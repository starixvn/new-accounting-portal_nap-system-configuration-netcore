namespace NapAdminCore.Criteria.Group;

using Fanex.Data.Repository;

public class GetByUserGroupCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public override string GetSettingKey() => "NAP_Group_GetByUser";

    public override bool IsValid() => true;
}
