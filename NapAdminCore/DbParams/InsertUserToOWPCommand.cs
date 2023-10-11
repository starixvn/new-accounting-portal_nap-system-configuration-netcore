namespace NapAdminCore.DbParams;

using Fanex.Data.Repository;

public class InsertUserToOWPCommand : NonQueryCommand
{
    public InsertUserToOWPCommand(int userId)
    {
        UserId = userId;
        ErrCode = 0;
        ErrMsg = string.Empty;
    }

    public int UserId { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "OWP_UserInsert";

    public override bool IsValid() => UserId > 0;
}
