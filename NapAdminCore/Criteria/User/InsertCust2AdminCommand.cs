namespace NapAdminCore.Criteria.User;

using Fanex.Data.Repository;

public class InsertCust2AdminCommand : NonQueryCommand
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string UserName { get; set; }

    public string UserPwd { get; set; }
    
    public int CustId { get; set; }

    public int CurrentUserId { get; set; }

    public int ErrCode { get; set; } = 0;

    public string ErrMsg { get; set; } = string.Empty;


    public override string GetSettingKey() => "NAP_Admin_Ins";

    public override bool IsValid() => true;
}
