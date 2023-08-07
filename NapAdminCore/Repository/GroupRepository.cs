
namespace NapAdminCore.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using Command;
using Criteria.Group;
using Criteria.Misc;
using Fanex.Data;
using Fanex.Data.Repository;
using Models;
using Models.ValueObject;
using Helpers;

public interface IGroupRepository
{
    ICommandResult Insert(InsertGroupCriteria criteria, ICriteria logCriteria);

    ICommandResult Update(ICriteria criteria, ICriteria logCriteria);

    IEnumerable<GroupPolicy> GetGroupPolicy(ICriteria criteria);
}
public class GroupRepository : BaseRepository, IGroupRepository
{
    private readonly IDynamicRepository dynamicRepository;

    public GroupRepository(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public Group Get(ICriteria criteria)
    {
        return dynamicRepository.Get<Group>(criteria);
    }

    public IEnumerable<Group> Fetch(ICriteria criteria)
    {
        return dynamicRepository.Fetch<Group>(criteria);
    }

    public ICommandResult Insert(InsertGroupCriteria criteria, ICriteria logCriteria)
    {
        var hashtable = HashtableExtensions.ToHashtable(criteria, logCriteria, new ResultCriteria());

        using var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        objectDb.ExecuteNonQuery(hashtable);
        criteria.GroupId = Convert.ToInt32(objectDb.GetParameterValue("GroupId"));

        return GetCommandResult(hashtable);
    }

    public ICommandResult Update(ICriteria criteria, ICriteria logCriteria)
    {
        var hashtable = HashtableExtensions.ToHashtable(criteria, logCriteria, new ResultCriteria());

        using var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        objectDb.ExecuteNonQuery(hashtable);

        return GetCommandResult(hashtable);
    }

    public IEnumerable<GroupPolicy> GetGroupPolicy(ICriteria criteria)
    {
        var userLevelList = dynamicRepository.Fetch<GroupPolicy>(criteria);
        return userLevelList.OrderBy(g => g.Name).ToList();
    }
}
