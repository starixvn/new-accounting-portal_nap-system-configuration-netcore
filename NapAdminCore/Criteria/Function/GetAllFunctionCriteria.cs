namespace NapAdminCore.Criteria.Function;

using Fanex.Data.Repository;

public class GetAllFunctionCriteria : CriteriaBase
{
    public string Environment { get; set; }

    public bool? IsActive { get; set; }

    public override string GetSettingKey() => "NAP_Function_GetAll";

    public override bool IsValid() => true;
}
