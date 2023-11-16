
namespace NapAdminCore.Repository;

using System.Collections.Generic;
using System.Linq;
using Fanex.Data.Repository;
using Models.ValueObject;

public interface IGroupRepository:IBaseRepository
{
    IEnumerable<GroupPolicy> GetGroupPolicy(ICriteria criteria);
}

public class GroupRepository : BaseRepository, IGroupRepository
{
    public IEnumerable<GroupPolicy> GetGroupPolicy(ICriteria criteria)
    {
        var userLevelList = dynamicRepository.Fetch<GroupPolicy>(criteria);
        return userLevelList.OrderBy(g => g.Name).ToList();
    }
}
