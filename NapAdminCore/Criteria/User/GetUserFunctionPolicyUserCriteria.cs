namespace NapAdminCore.Criteria.User;

using Fanex.Data.Repository;

public class GetUserFunctionPolicyUserCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public int FunctionId { get; set; }

    public override string GetSettingKey() => "NAP_User_GetUserFunctionPolicy";

    public override bool IsValid() => true;
}
