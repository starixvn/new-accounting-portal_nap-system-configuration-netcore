namespace NapAdminCore.Criteria.Function;

using Fanex.Data.Repository;

public class GetByUserAndSiteIncludeGroupFunctionCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public int SiteId { get; set; }

    public string Environment { get; set; }

    public override string GetSettingKey() => "NAP_Function_GetByUserAndSiteIncludeGroup";

    public override bool IsValid() => true;
}
