
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
    private readonly IDynamicRepository dynamicRepository;

    public BaseRepository()
    {
    }

    public BaseRepository(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

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
