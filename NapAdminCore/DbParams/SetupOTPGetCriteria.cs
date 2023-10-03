namespace NapAdminCore.DbParams;

using Fanex.Data.Repository;

public class SetupOTPGetCriteria : CriteriaBase
{
    public int CustId { get; set; }

    public string System { get; set; }

    public override string GetSettingKey() => "NAP_Admin_GetOTP";

    public override bool IsValid() => true;
}
