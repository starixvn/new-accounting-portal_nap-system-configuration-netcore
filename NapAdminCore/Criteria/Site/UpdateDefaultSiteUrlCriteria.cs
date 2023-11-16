namespace NapAdminCore.Criteria.Site;

public class UpdateDefaultSiteUrlCriteria : UpdateCommandBase
{
    public int SiteUrlId { get; set; }

    public int SiteId { get; set; }

    public string Environment { get; set; }

    public override string GetSettingKey() => "NAP_Site_UpdateDefaultSiteURL";

    public override bool IsValid() => true;
}
