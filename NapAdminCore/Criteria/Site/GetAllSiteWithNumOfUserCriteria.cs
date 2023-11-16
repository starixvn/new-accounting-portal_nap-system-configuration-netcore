namespace NapAdminCore.Criteria.Site;

using Fanex.Data.Repository;

public class GetAllSiteWithNumOfUserCriteria : CriteriaBase
{
    public string Environment { get; set; }

    public bool? IsActive { get; set; }

    public override string GetSettingKey() => "NAP_Site_GetAllWithNumOfUser";

    public override bool IsValid() => true;
}
