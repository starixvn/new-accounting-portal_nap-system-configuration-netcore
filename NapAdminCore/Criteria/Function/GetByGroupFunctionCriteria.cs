namespace NapAdminCore.Criteria.Function;

using Fanex.Data.Repository;

public class GetByGroupFunctionCriteria : CriteriaBase
{
    public int GroupId { get; set; }

    public string Environment { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Function_GetByGroup";
    }

    public override bool IsValid()
    {
        return true;
    }
}
