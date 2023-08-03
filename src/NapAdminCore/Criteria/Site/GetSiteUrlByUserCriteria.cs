
namespace NapAdminCore.Criteria.Site;

using Fanex.Data.Repository;

public class GetSiteUrlByUserCriteria : CriteriaBase
{
    public int UserId { get; set; }
        
    public string Environment { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Site_GetSiteURLByUser";
    }

    public override bool IsValid()
    {
        return true;
    }
}
