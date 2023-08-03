
namespace NapAdminCore.Criteria.Site;

using Business;
using Fanex.Data.Repository;

public class GetAllSiteCriteria : CriteriaBase
{
    public GetAllSiteCriteria()
    {
        Environment = Constant.Uat;
    }

    public string Environment { get; set; }

    public bool? IsActive { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Site_GetAll";
    }

    public override bool IsValid()
    {
        return true;
    }
}
