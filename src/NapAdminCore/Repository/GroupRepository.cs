
namespace NapAdminCore.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using Command;
using Criteria.Group;
using Criteria.Misc;
using Fanex.Common.Extensions;
using Fanex.Data;
using Fanex.Data.Repository;
using Models;
using Models.ValueObject;
using Helpers;

public interface IGroupRepository
{
    ICommandResult Insert(InsertGroupCriteria criteria, ICriteria logCriteria);

    ICommandResult Update(ICriteria criteria, ICriteria logCriteria);

    ICommandResult Delete(ICriteria criteria, ICriteria logCriteria);

    IEnumerable<GroupPolicy> GetGroupPolicy(ICriteria criteria);
}
public class GroupRepository : BaseRepository<Group>, IGroupRepository
{
    public Group Get(ICriteria criteria)
    {
        using var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var objGroup = objectDb.QueryEntity<Group, User, User, Group>(
            (g, cu, mu) => { g.Creator = cu; g.Modifier = mu; return g; },
            "CreatorId,ModifierId",
            criteria);

        return objGroup;
    }

    public IEnumerable<Group> Fetch(ICriteria criteria)
    {
        using var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var listGroup = objectDb.Query<Group, User, User, Group>(
            (g, cu, mu) => { g.Creator = cu; g.Modifier = mu; return g; },
            "CreatorId,ModifierId",
            criteria);

        return listGroup.OrderBy(g => g.Name).ToList();
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

    public ICommandResult Delete(ICriteria criteria, ICriteria logCriteria)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<GroupPolicy> GetGroupPolicy(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var reader = db.ExecuteReader(criteria);
        var userLevelList = new List<GroupPolicy>();

        while (reader.Read())
        {
            userLevelList.Add(new GroupPolicy
            {
                Policy = reader.ToInt("Policy"),
                Group = reader.IsDBNull("GroupId") ? null : new Group
                {
                    Id = reader.ToShort("GroupId"),
                    Name = reader.ToString("GroupName"),
                    CreatedDate = reader.ToDateTime("CreatedDate"),
                    ModifiedDate = reader.ToDateTime("ModifiedDate"),
                    Creator = reader.IsDBNull("CreatorId") ? null : new User
                    {
                        Id = reader.ToInt("CreatorId"),
                        Username = reader.ToString("CreatorUsername")
                    },
                    Modifier = reader.IsDBNull("ModifierId") ? null : new User
                    {
                        Id = reader.ToInt("ModifierId"),
                        Username = reader.ToString("ModifierUsername")
                    }
                },
                CreatedDate = reader.ToDateTime("CreatedDateGroupFunction"),
                ModifiedDate = reader.ToDateTime("ModifiedDateGroupFunction"),
                Creator = reader.IsDBNull("CreatorIdGroupFunction") ? null : new User
                {
                    Id = reader.ToInt("CreatorIdGroupFunction"),
                    Username = reader.ToString("CreatorUsernameGroupFunction")
                },
                Modifier = reader.IsDBNull("ModifierIdGroupFunction") ? null : new User
                {
                    Id = reader.ToInt("ModifierIdGroupFunction"),
                    Username = reader.ToString("ModifierUsernameGroupFunction")
                }
            });
        }

        return userLevelList.OrderBy(g => g.Group.Name).ToList();
    }
}
