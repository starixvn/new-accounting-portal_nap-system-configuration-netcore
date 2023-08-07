namespace NapAdminCore.DbParams;

using Fanex.Data.Repository;

public class SetupOTPListCriteria : CriteriaBase
{
    public string Username { get; set; }

    public string System { get; set; }

    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public int TotalRows { get; set; }

    public int Status { get; set; }

    public override string GetSettingKey() => "NAP_Admin_OTPSel";

    public override bool IsValid() => true;
}
