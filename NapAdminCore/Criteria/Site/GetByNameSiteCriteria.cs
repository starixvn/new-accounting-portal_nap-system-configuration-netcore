namespace NapAdminCore.Criteria.Site;

using Fanex.Data.Repository;

public class GetByNameSiteCriteria : CriteriaBase
{
    public string SiteName { get; set; }

    public override string GetSettingKey() => "NAP_Site_GetByName";

    public override bool IsValid() => true;
}
