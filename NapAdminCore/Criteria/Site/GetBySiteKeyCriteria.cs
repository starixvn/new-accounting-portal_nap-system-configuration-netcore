namespace NapAdminCore.Criteria.Site;

using Fanex.Data.Repository;

public class GetBySiteKeyCriteria : CriteriaBase
{
    public string SiteKey { get; set; }

    public override bool IsValid() => !string.IsNullOrWhiteSpace(SiteKey);

    public override string GetSettingKey() => "NAP_Site_GetSiteKey";
}
