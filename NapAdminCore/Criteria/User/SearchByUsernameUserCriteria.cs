namespace NapAdminCore.Criteria.User;

using Fanex.Data.Repository;

public class SearchByUsernameUserCriteria : CriteriaBase
{
    public string Username { get; set; }

    public int? SiteId { get; set; }

    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public int TotalRows { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_SearchByUsername";
    }

    public override bool IsValid()
    {
        return true;
    }
}
