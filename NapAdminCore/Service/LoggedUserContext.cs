
namespace NapAdminCore.Service;

using Criteria.Misc;

public interface LoggedUserContext
{
    LogExecSpCriteria GetLogExecSpCriteria();

    string GetIp();
}
