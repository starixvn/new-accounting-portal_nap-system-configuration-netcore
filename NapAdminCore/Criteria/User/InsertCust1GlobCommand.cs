using Fanex.Data.Repository;

namespace NapAdminCore.Criteria.User;

public class InsertCust1GlobCommand : NonQueryCommand
{
    public string Username { get; set; }

    public int SiteDbGroupId => 0;

    public int RoleId => 6;

    public int CustId { get; set; }

    public int ErrCode { get; set; } = 0;

    public string ErrMsg { get; set; } = string.Empty;

    public override string GetSettingKey() => "Acc_CustomerIns_Register";

    public override bool IsValid() => true;
}
