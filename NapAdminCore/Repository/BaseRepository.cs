
namespace NapAdminCore.Repository;

using Command;
using Criteria;
using Criteria.Misc;
using Fanex.Data.Repository;


public interface IBaseRepository
{
    ICommandResult Execute(UpdateCommandBase command, LogExecSpCriteria logCriteria);
}

public class BaseRepository : DynamicRepository, IBaseRepository
{
    protected IDynamicRepository dynamicRepository = new DynamicRepository();

    public ICommandResult Execute(UpdateCommandBase command, LogExecSpCriteria logCriteria)
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
