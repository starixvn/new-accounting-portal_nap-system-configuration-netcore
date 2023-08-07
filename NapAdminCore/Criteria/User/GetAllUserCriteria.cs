namespace NapAdminCore.Criteria.User;

using Fanex.Data.Repository;

public class GetAllUserCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NAP_User_GetAll";
    }

    public override bool IsValid()
    {
        return true;
    }
}
