
namespace NapAdminCore.Criteria.Function;

using Fanex.Data.Repository;

public class GetByUserAndFunctionKeyIncludeGroupFunctionCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public string FunctionKey { get; set; }
        
    public string Environment { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Function_GetByUserAndFunctionKeyIncludeGroup";
    }

    public override bool IsValid()
    {
        return true;
    }
}
