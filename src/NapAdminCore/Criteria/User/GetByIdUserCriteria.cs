
namespace NapAdminCore.Criteria.User;

using Fanex.Data.Repository;

public class GetByIdUserCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_GetById";
    }

    public override bool IsValid()
    {
        return true;
    }
}
