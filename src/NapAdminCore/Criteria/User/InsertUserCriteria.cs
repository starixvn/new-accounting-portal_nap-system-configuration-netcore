
namespace NapAdminCore.Criteria.User;

using System;
using Fanex.Data.Repository;

public class InsertUserCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public int GroupId { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_Insert";
    }

    public override bool IsValid()
    {
        return true;
    }
}
