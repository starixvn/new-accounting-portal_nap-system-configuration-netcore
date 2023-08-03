
namespace NapAdminCore.Criteria.Site;

using Fanex.Data.Repository;

public class UpdateStatusSiteCriteria : CriteriaBase
{
    public int SiteId { get; set; }

    public bool IsActive { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Site_UpdateStatus";
    }

    public override bool IsValid()
    {
        return true;
    }
}
