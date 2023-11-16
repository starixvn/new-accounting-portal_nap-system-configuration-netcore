namespace NapAdminCore.Criteria.User;

using System;

public class InsertUserCriteria : UpdateCommandBase
{
    public int UserId { get; set; }

    public int GroupId { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public override string GetSettingKey() => "NAP_User_Insert";

    public override bool IsValid() => true;
}
