namespace NapAdminCore.Criteria.Site;

using Fanex.Data.Repository;
using Models;

public class GetAllSiteCriteria : CriteriaBase
{
    public string Environment { get; set; } = AdminConstant.Uat;

    public bool? IsActive { get; set; }

    public override string GetSettingKey() => "NAP_Site_GetAll";

    public override bool IsValid() => true;
}
