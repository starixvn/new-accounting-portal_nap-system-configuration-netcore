namespace NapAdminCore.Criteria.User;

using System;
using Fanex.Data.Repository;

public class UserSSOGetTimeUserCriteria : CriteriaBase
{
    public DateTime Time { get; set; }

    public override string GetSettingKey() => "NAP_User_SSOGetTime";

    public override bool IsValid() => true;
}
