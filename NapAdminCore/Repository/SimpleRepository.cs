
namespace NapAdminCore.Repository;

using Criteria;
using Criteria.Misc;
using Fanex.Data.Repository;
using Models.ValueObject;

public interface ISimpleRepository
{
    CommandResult Execute(UpdateCommandBase command, LogExecSpCriteria logCriteria);

    IList<FunctionPolicy> FetchFunctionPolicy(ICriteria criteria);

    IList<GroupPolicy> GetGroupPolicy(ICriteria criteria);
}

public class SimpleSimpleRepository : ISimpleRepository
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

    public IList<FunctionPolicy> FetchFunctionPolicy(ICriteria criteria)
    {
        var policies = dynamicRepository.Fetch<FunctionPolicy>(criteria);
        return policies.OrderBy(f => f.Name).ToList();
    }

    public IList<GroupPolicy> GetGroupPolicy(ICriteria criteria)
    {
        var userLevelList = dynamicRepository.Fetch<GroupPolicy>(criteria);
        return userLevelList.OrderBy(g => g.Name).ToList();
    }
}
