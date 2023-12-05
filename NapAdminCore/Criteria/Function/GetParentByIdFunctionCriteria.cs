namespace NapAdminCore.Criteria.Function;

using Fanex.Data.Repository;

public class GetParentByIdFunctionCriteria : CriteriaBase
{
    public int FunctionId { get; set; }

    public override string GetSettingKey() => "NAP_Function_GetParentById";

    public override bool IsValid() => true;
}
