namespace NapAdminCore.Criteria.Group;

using Fanex.Data.Repository;

public class GetAllGroupCriteria : CriteriaBase
{
    public bool? IsActive { get; set; }

    public override string GetSettingKey() => "NAP_Group_GetAll";

    public override bool IsValid() => true;
}
