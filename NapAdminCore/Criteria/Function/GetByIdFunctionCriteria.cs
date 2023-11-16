namespace NapAdminCore.Criteria.Function;

using Fanex.Data.Repository;

public class GetByIdFunctionCriteria : CriteriaBase
{
    public int FunctionId { get; set; }

    public override string GetSettingKey() => "NAP_Function_GetById";

    public override bool IsValid() => true;
}
