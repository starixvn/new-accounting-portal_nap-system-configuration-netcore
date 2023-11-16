namespace NapAdminCore.Criteria.Function;

using Fanex.Data.Repository;

public class GetByNameAndSiteFunctionCriteria : CriteriaBase
{
    public string FunctionName { get; set; }

    public int SiteId { get; set; }

    public override string GetSettingKey() => "NAP_Function_GetByNameAndSite";

    public override bool IsValid() => true;
}
