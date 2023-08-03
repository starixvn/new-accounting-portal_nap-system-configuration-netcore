
namespace NapAdminCore.Criteria.Function;

using Fanex.Data.Repository;

public class GetByUserFunctionCriteria : CriteriaBase
{
    public int UserId { get; set; }
        
    public string Environment { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Function_GetByUser";
    }

    public override bool IsValid()
    {
        return true;
    }
}
