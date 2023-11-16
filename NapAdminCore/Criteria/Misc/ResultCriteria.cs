namespace NapAdminCore.Criteria.Misc;

using Fanex.Data.Repository;

public class ResultCriteria : CriteriaBase
{
    public int ErrCode { get; set; } = 0;

    public string ErrMsg { get; set; } = string.Empty;

    public override string GetSettingKey() => string.Empty;

    public override bool IsValid() => true;
}
