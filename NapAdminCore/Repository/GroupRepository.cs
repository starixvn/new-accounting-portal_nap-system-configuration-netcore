
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
    private readonly IDynamicRepository dynamicRepository;

    public GroupRepository(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public IEnumerable<GroupPolicy> GetGroupPolicy(ICriteria criteria)
    {
        var userLevelList = dynamicRepository.Fetch<GroupPolicy>(criteria);
        return userLevelList.OrderBy(g => g.Name).ToList();
    }
}
