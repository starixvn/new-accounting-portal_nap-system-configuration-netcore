
namespace NapAdminCore.Criteria.Function;

using Fanex.Data.Repository;

public class GetByUserAndSiteFunctionCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public int SiteId { get; set; }
        
    public string Environment { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Function_GetByUserAndSite";
    }

    public override bool IsValid()
    {
        return true;
    }
}
