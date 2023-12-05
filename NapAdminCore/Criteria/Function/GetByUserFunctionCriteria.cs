namespace NapAdminCore.Criteria.Function;

using Fanex.Data.Repository;

public class GetByUserFunctionCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public string Environment { get; set; }

    public override string GetSettingKey() => "NAP_Function_GetByUser";

    public override bool IsValid() => true;
}
