
namespace NapAdminCore.Criteria.Group;

using System;
using Fanex.Data.Repository;

public class UpdateStatusGroupCriteria : CriteriaBase
{
    public int GroupId { get; set; }

    public bool IsActive { get; set; }

    public int ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Group_UpdateStatus";
    }

    public override bool IsValid()
    {
        return true;
    }
}
