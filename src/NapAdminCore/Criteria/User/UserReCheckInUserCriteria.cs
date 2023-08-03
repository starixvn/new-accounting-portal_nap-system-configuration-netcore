
namespace NapAdminCore.Criteria.User;

using Fanex.Data.Repository;

public class UserReCheckInUserCriteria : CriteriaBase
{       
    public int OnUserId { get; set; }

    public bool IsKickedOut { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_ReCheckIn";
    }

    public override bool IsValid()
    {
        return true;
    }
}
