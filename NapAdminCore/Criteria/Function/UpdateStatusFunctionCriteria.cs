namespace NapAdminCore.Criteria.Function;

using System;
using Fanex.Data.Repository;

public class UpdateStatusFunctionCriteria : CriteriaBase
{
    public int FunctionId { get; set; }

    public bool IsActive { get; set; }

    public int ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Function_UpdateStatus";
    }

    public override bool IsValid()
    {
        return true;
    }
}
