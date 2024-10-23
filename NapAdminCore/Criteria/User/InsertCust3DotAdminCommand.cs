namespace NapAdminCore.Criteria.User;

using Fanex.Data.Repository;

public class InsertCust3DotAdminCommand : NonQueryCommand
{
    public int CustID { get; set; }

    public string UserName { get; set; }

    public string UserPWD { get; set; }

    public int Creator { get; set; }

    public override string GetSettingKey() => "Dot_Admin_Add_AdminSiteUser";

    public override bool IsValid() => true;
}
