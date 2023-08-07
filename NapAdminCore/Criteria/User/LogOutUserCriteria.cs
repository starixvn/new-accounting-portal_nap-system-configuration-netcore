namespace NapAdminCore.Criteria.User;

using Fanex.Data.Repository;

public class LogOutUserCriteria : CriteriaBase
{
    public int OnUserId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_Logout";
    }

    public override bool IsValid()
    {
        return true;
    }
}
