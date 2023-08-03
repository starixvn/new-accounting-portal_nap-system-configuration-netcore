
namespace NapAdminCore.Criteria.Trace;

using Fanex.Data.Repository;

public class GetByIdLogCriteria : CriteriaBase
{
    public int LogId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_LogExecSp_GetById";
    }

    public override bool IsValid()
    {
        return true;
    }
}
