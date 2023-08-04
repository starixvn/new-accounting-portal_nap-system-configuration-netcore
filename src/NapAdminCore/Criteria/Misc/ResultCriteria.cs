namespace NapAdminCore.Criteria.Misc;

using Fanex.Data.Repository;

public class ResultCriteria : CriteriaBase
{
    public ResultCriteria()
    {
        this.ErrCode = 0;
        this.ErrMsg = string.Empty;
    }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => string.Empty;

    public override bool IsValid() => true;
}
