namespace NapAdminCore.Criteria.Site;

using Fanex.Data.Repository;

public class GetByUserSiteCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public string Environment { get; set; }

    public bool? IsActive { get; set; }

    public override string GetSettingKey() => "NAP_Site_GetByUser";

    public override bool IsValid() => true;
}
