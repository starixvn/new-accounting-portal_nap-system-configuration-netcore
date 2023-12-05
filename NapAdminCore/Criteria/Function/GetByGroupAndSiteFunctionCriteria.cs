namespace NapAdminCore.Criteria.Function;

using Fanex.Data.Repository;

public class GetByGroupAndSiteFunctionCriteria : CriteriaBase
{
    public int GroupId { get; set; }

    public int SiteId { get; set; }

    public string Environment { get; set; }

    public override string GetSettingKey() => "NAP_Function_GetByGroupAndSite";

    public override bool IsValid() => true;
}
