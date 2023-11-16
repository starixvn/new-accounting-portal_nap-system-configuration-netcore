namespace NapAdminCore.Criteria.Site;

public class UpdateStatusSiteCriteria : UpdateCommandBase
{
    public int SiteId { get; set; }

    public bool IsActive { get; set; }

    public override string GetSettingKey() => "NAP_Site_UpdateStatus";

    public override bool IsValid() => true;
}
