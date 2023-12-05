namespace NapAdminCore.Criteria.Function;

using Fanex.Data.Repository;

public class GetByParentKeyAndSiteCriteria : CriteriaBase
{
    public int SiteId { get; set; }

    public string ParentKey { get; set; }

    public string Environment { get; set; }

    public bool? IsActive { get; set; }

    public int? RoleId { get; set; }

    public int? UserId { get; set; }

    public override string GetSettingKey() => "NAP_Function_GetByParentKeyAndSite";

    public override bool IsValid() => SiteId > 0;
}
