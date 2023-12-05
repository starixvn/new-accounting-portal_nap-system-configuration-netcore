namespace NapAdminCore.Criteria.User;

using Fanex.Data.Repository;

public class InsertUserToOWPCommand(int userId) : NonQueryCommand
{
    public int UserId { get; set; } = userId;

    public int ErrCode { get; set; } = 0;

    public string ErrMsg { get; set; } = string.Empty;

    public override string GetSettingKey() => "OWP_UserInsert";

    public override bool IsValid() => UserId > 0;
}
