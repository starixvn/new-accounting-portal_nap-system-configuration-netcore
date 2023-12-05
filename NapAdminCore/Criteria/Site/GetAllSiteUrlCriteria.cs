namespace NapAdminCore.Criteria.Site;

using Fanex.Data.Repository;

public class GetAllSiteUrlCriteria : CriteriaBase
{
    public string Environment { get; set; }

    public override string GetSettingKey() => "NAP_Site_GetAllSiteUrl";

    public override bool IsValid() => true;
}
