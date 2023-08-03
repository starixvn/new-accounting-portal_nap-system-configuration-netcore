
namespace NapAdminCore.Repository;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Command;
using Criteria.Misc;
using Criteria.User;
using Domain;
using Fanex.Common.Extensions;
using Fanex.Data;
using Fanex.Data.Repository;
using Models;
using Models.ValueObject;
using Helpers;

public interface IUserRepository 
{
    IQueryable<IUser> SearchByUsername(SearchByUsernameUserCriteria criteria);

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

public class UserRepository : BaseRepository<IUser>, IUserRepository
{
    public  IUser Get(ICriteria criteria)
    {
        using var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var objUser = objectDb.QueryEntity<User, Group, User, User, User>(
            (u, g, uc, um) => { u.Group = g; u.Creator = uc; u.Modifier = um; return u; },
            "GroupId,CreatorId,ModifierId",
            criteria);

        return objUser;
    }

    public  IEnumerable<IUser> Fetch(ICriteria criteria)
    {
        using var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var objListUser = objectDb.Query<User, Group, User, User, User>(
            (u, g, uc, um) => { u.Group = g; u.Creator = uc; u.Modifier = um; return u; },
            "GroupId,CreatorId,ModifierId",
            criteria);

        return objListUser;
    }

    public IQueryable<IUser> SearchByUsername(SearchByUsernameUserCriteria criteria)
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
                    Group = reader.IsDBNull("GroupId") ? null : new Group
                    {
                        Id = reader.ToShort("GroupId"),
                        Name = reader.ToString("GroupName")
                    },
                    Creator = reader.IsDBNull("CreatorId") ? null : new User
                    {
                        Id = reader.ToInt("CreatorId"),
                        Username = reader.ToString("CreatorUsername")
                    },
                    Modifier = reader.IsDBNull("ModifierId") ? null : new User
                    {
                        Id = reader.ToInt("ModifierId"),
                        Username = reader.ToString("ModifierUsername")
                    },
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
                    Group = reader.IsDBNull("GroupId") ? null : new Group
                    {
                        Id = reader.ToShort("GroupId"),
                        Name = reader.ToString("GroupName")
                    },
                    Creator = reader.IsDBNull("CreatorId") ? null : new User
                    {
                        Id = reader.ToInt("CreatorId"),
                        Username = reader.ToString("CreatorUsername")
                    },
                    Modifier = reader.IsDBNull("ModifierId") ? null : new User
                    {
                        Id = reader.ToInt("ModifierId"),
                        Username = reader.ToString("ModifierUsername")
                    },
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
                Group = reader.IsDBNull("GroupId") ? null : new Group
                {
                    Id = reader.ToShort("GroupId"),
                    Name = reader.ToString("GroupName")
                },
                Creator = reader.IsDBNull("CreatorId") ? null : new User
                {
                    Id = reader.ToInt("CreatorId"),
                    Username = reader.ToString("CreatorUsername")
                },
                Modifier = reader.IsDBNull("ModifierId") ? null : new User
                {
                    Id = reader.ToInt("ModifierId"),
                    Username = reader.ToString("ModifierUsername")
                },
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
            LastLoginDate = reader.ToDateTime("LastLoginDateUserSite"),
            Site = reader.IsDBNull("SiteId") ? null : new Site
            {
                Id = reader.ToByte("SiteId"),
                Name = reader.ToString("SiteName")
            }
        };
    }
}
