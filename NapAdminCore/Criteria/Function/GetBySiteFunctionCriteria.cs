namespace NapAdminCore.Criteria.Function;

using Fanex.Data.Repository;

public class GetBySiteFunctionCriteria : CriteriaBase
{
    public int SiteId { get; set; }

    public string Environment { get; set; }

    public bool? IsActive { get; set; }

    public override string GetSettingKey() => "NAP_Function_GetBySite";

    public override bool IsValid() => true;
}
