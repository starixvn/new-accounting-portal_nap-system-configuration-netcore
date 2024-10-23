namespace NapAdminCore.Criteria.User;

public class InsertCust4Command : UpdateCommandBase
{
    public int CustId { get; set; }

    public string Username { get; set; }

    public string UserPwd { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public int GroupId { get; set; }

    public string LoginIP { get; set; }

    public string Language { get; set; }

    public override string GetSettingKey() => "NAP_Cust_Insert";

    public override bool IsValid() => true;
}
