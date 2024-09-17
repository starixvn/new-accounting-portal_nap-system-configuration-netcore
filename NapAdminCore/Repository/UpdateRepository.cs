
namespace NapAdminCore.Repository;

using Criteria;
using Criteria.Misc;
using Fanex.Data.Repository;

public interface UpdateRepository
{
    CommandResult Execute(UpdateCommandBase command, LogExecSpCriteria logCriteria);
}

public class UpdateRepositoryImp : UpdateRepository
{
    protected IDynamicRepository dynamicRepository = new DynamicRepository();

    public CommandResult Execute(UpdateCommandBase command, LogExecSpCriteria logCriteria)
    {
        command.CopyFromLogCriteria(logCriteria);

        dynamicRepository.Execute(command);

        return new CommandResult
        {
            Code = command.ErrCode,
            Message = command.ErrMsg
        };
    }
}
