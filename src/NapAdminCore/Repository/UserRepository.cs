
namespace NapAdminCore.Repository;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Command;
using Criteria.Misc;
using Criteria.User;
using Fanex.Common.Extensions;
using Fanex.Data;
using Fanex.Data.Repository;
using Models;
using Models.ValueObject;
using Helpers;

public interface IUserRepository
{
    IQueryable<User> SearchByUsername(SearchByUsernameUserCriteria criteria);

    IEnumerable<UserLevel> Search(ICriteria criteria);

    IQueryable<UserLevel> Search(SearchUserCriteria criteria);

    IQueryable<UserLevel> GetAllByUsernameGroupSite(GetAllByUsernameGroupSiteUserCriteria criteria);

    IQueryable<UserLevel> SearchUserExcludeFunction(SearchUserExcludeFunctionCriteria criteria);

    IEnumerable<UserPolicy> GetUserPolicy(ICriteria criteria);

    ICommandResult Insert(InsertCustCriteria criteria, ICriteria logCriteria);

    ICommandResult Update(ICriteria criteria, ICriteria logCriteria);

    void UserCheckIn(UserCheckInUserCriteria criteria);

    void UserSSOCheckIn(UserSSOCheckInUserCriteria criteria);

    void UserReCheckIn(UserReCheckInUserCriteria criteria);

    void UserLogOut(LogOutUserCriteria criteria);

    Hashtable GetUserListSiteFunctionKeyPolicy(ICriteria criteria);

    string UserSSOGetTime(ICriteria criteria);
}

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly IDynamicRepository dynamicRepository;

    public UserRepository(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public User Get(ICriteria criteria)
    {
        return dynamicRepository.Get<User>(criteria);
    }

    public IEnumerable<User> Fetch(ICriteria criteria)
    {
        return dynamicRepository.Fetch<User>(criteria);
    }

    public IQueryable<User> SearchByUsername(SearchByUsernameUserCriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var hashtable = HashtableExtensions.ToHashtable(criteria);

        var userList = db.Query<User>(hashtable);

        criteria.TotalRows = Convert.ToInt32(hashtable["totalrows"]);

        return userList.AsQueryable();
    }

    public ICommandResult Insert(InsertCustCriteria criteria, ICriteria logCriteria)
    {
        var hashtable = HashtableExtensions.ToHashtable(criteria, logCriteria, new ResultCriteria());

        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        db.ExecuteNonQuery(hashtable);
        criteria.CustId = Convert.ToInt32(db.GetParameterValue("CustId"));

        return GetCommandResult(hashtable);
    }

    public ICommandResult Update(ICriteria criteria, ICriteria logCriteria)
    {
        var hashtable = HashtableExtensions.ToHashtable(criteria, logCriteria, new ResultCriteria());

        using var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        objectDb.ExecuteNonQuery(hashtable);
        return GetCommandResult(hashtable);
    }

    public void UserCheckIn(UserCheckInUserCriteria criteria)
    {
        var hashtable = HashtableExtensions.ToHashtable(criteria);

        using var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        objectDb.ExecuteNonQuery(hashtable);
        criteria.OnUserId = Convert.ToInt32(hashtable["onuserid"]?.ToString());
    }

    public void UserReCheckIn(UserReCheckInUserCriteria criteria)
    {
        var hashtable = HashtableExtensions.ToHashtable(criteria);

        using var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        objectDb.ExecuteNonQuery(hashtable);
        criteria.IsKickedOut = Convert.ToBoolean(hashtable["iskickedout"]?.ToString());
    }

    public void UserLogOut(LogOutUserCriteria criteria)
    {
        var hashtable = HashtableExtensions.ToHashtable(criteria);

        using var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        objectDb.ExecuteNonQuery(hashtable);
    }

    public void UserSSOCheckIn(UserSSOCheckInUserCriteria criteria)
    {
        var hashtable = HashtableExtensions.ToHashtable(criteria);

        using var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        objectDb.ExecuteNonQuery(hashtable);
        criteria.OnUserId = Convert.ToInt32(hashtable["onuserid"]?.ToString());
    }

    public IEnumerable<UserLevel> Search(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var reader = db.ExecuteReader(criteria);
        var userLevelList = new List<UserLevel>();

        while (reader.Read())
        {
            userLevelList.Add(new UserLevel
            {
                Level = reader.ToInt("Level"),
                User = reader.IsDBNull("UserId") ? null : new User
                {
                    Id = reader.ToInt("UserId"),
                    FirstName = reader.ToString("FirstName"),
                    LastName = reader.ToString("LastName"),
                    Username = reader.ToString("Username"),
                    Email = reader.ToString("Email"),
                    Password = reader.ToString("UserPwd"),
                    LastLoginDate = reader.ToDateTime("LastLoginDate"),
                    CreatedDate = reader.ToDateTime("CreatedDate"),
                    ModifiedDate = reader.ToDateTime("ModifiedDate"),
                    GroupId = reader.ToShort("GroupId"),
                    GroupName = reader.ToString("GroupName"),
                    CreatorId = reader.ToInt("CreatorId"),
                    CreatorUsername = reader.ToString("CreatorUsername"),
                    ModifierId = reader.ToInt("ModifierId"),
                    ModifierUsername = reader.ToString("ModifierUsername")
                },
                CreatedDate = reader.ToDateTime("CreatedDateUserSite"),
                ModifiedDate = reader.ToDateTime("ModifiedDateUserSite"),
                Creator = reader.IsDBNull("CreatorIdUserSite") ? null : new User
                {
                    Id = reader.ToInt("CreatorIdUserSite"),
                    Username = reader.ToString("CreatorUsernameUserSite")
                },
                Modifier = reader.IsDBNull("ModifierIdUserSite") ? null : new User
                {
                    Id = reader.ToInt("ModifierIdUserSite"),
                    Username = reader.ToString("ModifierUsernameUserSite")
                },
                IsActive = reader.ToBool("IsActive"),
                LastLoginDate = reader.ToDateTime("LastLoginDateUserSite")
            });
        }

        return userLevelList.GroupBy(u => u.User.Id).Select(u => u.First());
    }

    public IQueryable<UserLevel> Search(SearchUserCriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var reader = db.ExecuteReader(criteria);
        var userLevelList = new List<UserLevel>();

        while (reader.Read())
        {
            var userLevel = ReadUserLevel(reader);

            userLevelList.Add(userLevel);
        }

        reader.Close();

        criteria.TotalRows = (int)db.GetParameterValue("totalrows");

        return userLevelList.AsQueryable();
    }

    public IQueryable<UserLevel> GetAllByUsernameGroupSite(GetAllByUsernameGroupSiteUserCriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var reader = db.ExecuteReader(criteria);
        var userLevelList = new List<UserLevel>();

        while (reader.Read())
        {
            var userLevel = ReadUserLevel(reader);

            userLevelList.Add(userLevel);
        }

        reader.Close();

        criteria.TotalRows = (int)db.GetParameterValue("totalrows");

        return userLevelList.AsQueryable();
    }

    public IQueryable<UserLevel> SearchUserExcludeFunction(SearchUserExcludeFunctionCriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var reader = db.ExecuteReader(criteria);
        var userLevelList = new List<UserLevel>();

        while (reader.Read())
        {
            var userLevel = ReadUserLevel(reader);

            userLevelList.Add(userLevel);
        }

        reader.Close();

        criteria.TotalRows = (int)db.GetParameterValue("totalrows");

        return userLevelList.AsQueryable();
    }

    public IEnumerable<UserPolicy> GetUserPolicy(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var reader = db.ExecuteReader(criteria);
        var userLevelList = new List<UserPolicy>();

        while (reader.Read())
        {
            userLevelList.Add(new UserPolicy
            {
                Policy = reader.ToInt("Policy"),
                User = reader.IsDBNull("UserId") ? null : new User
                {
                    Id = reader.ToInt("UserId"),
                    FirstName = reader.ToString("FirstName"),
                    LastName = reader.ToString("LastName"),
                    Username = reader.ToString("Username"),
                    Email = reader.ToString("Email"),
                    Password = reader.ToString("UserPwd"),
                    LastLoginDate = reader.ToDateTime("LastLoginDate"),
                    CreatedDate = reader.ToDateTime("CreatedDate"),
                    ModifiedDate = reader.ToDateTime("ModifiedDate"),
                    GroupId = reader.ToShort("GroupId"),
                    GroupName = reader.ToString("GroupName"),
                    CreatorId = reader.ToInt("CreatorId"),
                    CreatorUsername = reader.ToString("CreatorUsername"),
                    ModifierId = reader.ToInt("ModifierId"),
                    ModifierUsername = reader.ToString("ModifierUsername")
                },
                CreatedDate = reader.ToDateTime("CreatedDateUserFunction"),
                ModifiedDate = reader.ToDateTime("ModifiedDateUserFunction"),
                Creator = reader.IsDBNull("CreatorIdUserFunction") ? null : new User
                {
                    Id = reader.ToInt("CreatorIdUserFunction"),
                    Username = reader.ToString("CreatorUsernameUserFunction")
                },
                Modifier = reader.IsDBNull("ModifierIdUserFunction") ? null : new User
                {
                    Id = reader.ToInt("ModifierIdUserFunction"),
                    Username = reader.ToString("ModifierUsernameUserFunction")
                }
            });
        }

        return userLevelList;
    }

    public Hashtable GetUserListSiteFunctionKeyPolicy(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var reader = db.ExecuteReader(criteria);
        var hashTable = new Hashtable();

        while (reader.Read())
        {
            var siteId = reader.ToByte("SiteId");
            if (!hashTable.ContainsKey(siteId))
            {
                hashTable.Add(siteId, reader.ToInt("Policy"));
            }
        }

        return hashTable;
    }

    public string UserSSOGetTime(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var hashtable = HashtableExtensions.ToHashtable(criteria);

        db.ExecuteNonQuery(hashtable);

        return hashtable["time"]?.ToString();
    }

    private UserLevel ReadUserLevel(IDataReader reader)
    {
        return new UserLevel
        {
            Level = reader.ToInt("Level"),
            User = reader.IsDBNull("UserId") ? null : new User
            {
                Id = reader.ToInt("UserId"),
                FirstName = reader.ToString("FirstName"),
                LastName = reader.ToString("LastName"),
                Username = reader.ToString("Username"),
                Email = reader.ToString("Email"),
                Password = reader.ToString("UserPwd"),
                LastLoginDate = reader.ToDateTime("LastLoginDate"),
                CreatedDate = reader.ToDateTime("CreatedDate"),
                ModifiedDate = reader.ToDateTime("ModifiedDate"),
                GroupId = reader.ToShort("GroupId"),
                GroupName = reader.ToString("GroupName"),
                CreatorId = reader.ToInt("CreatorId"),
                CreatorUsername = reader.ToString("CreatorUsername"),
                ModifierId = reader.ToInt("ModifierId"),
                ModifierUsername = reader.ToString("ModifierUsername")
            },
            CreatedDate = reader.ToDateTime("CreatedDateUserSite"),
            ModifiedDate = reader.ToDateTime("ModifiedDateUserSite"),
            Creator = reader.IsDBNull("CreatorIdUserSite") ? null : new User
            {
                Id = reader.ToInt("CreatorIdUserSite"),
                Username = reader.ToString("CreatorUsernameUserSite")
            },
            Modifier = reader.IsDBNull("ModifierIdUserSite") ? null : new User
            {
                Id = reader.ToInt("ModifierIdUserSite"),
                Username = reader.ToString("ModifierUsernameUserSite")
            },
            LastLoginDate = reader.ToDateTime("LastLoginDateUserSite"),
            Site = reader.IsDBNull("SiteId") ? null : new Site
            {
                Id = reader.ToByte("SiteId"),
                Name = reader.ToString("SiteName")
            },
            Status = TryGetColumnValue(reader, "Status", 0)
        };
    }

    private static T TryGetColumnValue<T>(IDataReader reader, string columnName, T defaultValue)
    {
        try
        {
            return reader.GetValue<T>(columnName);
        }
        catch
        {
            return defaultValue;
        }
    }
}
