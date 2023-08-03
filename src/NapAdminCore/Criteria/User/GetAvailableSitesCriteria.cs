
namespace NapAdminCore.Criteria.User;

using Fanex.Data.Repository;

public class GetAvailableSitesCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public string Environment { get; set; }

    public bool? IsActive { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_GetAvailableSites";
    }

    public override bool IsValid()
    {
        return true;
    }
}
