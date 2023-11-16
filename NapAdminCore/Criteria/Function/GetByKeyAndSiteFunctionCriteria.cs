namespace NapAdminCore.Criteria.Function;

using Fanex.Data.Repository;

public class GetByKeyAndSiteFunctionCriteria : CriteriaBase
{
    public string FunctionKey { get; set; }

    public int SiteId { get; set; }

    public override string GetSettingKey() => "NAP_Function_GetByKeyAndSite";

    public override bool IsValid() => true;
}
