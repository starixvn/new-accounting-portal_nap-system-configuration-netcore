namespace NapAdminCore.Criteria.Site;

using Fanex.Data.Repository;
using Models;

public class GetAllSiteCriteria : CriteriaBase
{
    public GetAllSiteCriteria()
    {
        this.Environment = AdminConstant.Uat;
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
