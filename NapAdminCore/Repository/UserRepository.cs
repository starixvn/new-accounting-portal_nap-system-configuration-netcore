
namespace NapAdminCore.Repository;

using System;
using System.Collections;
using System.Collections.Generic;
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


    IQueryable<UserLevel> Search(SearchUserCriteria criteria);

    IQueryable<UserLevel> GetAllByUsernameGroupSite(GetAllByUsernameGroupSiteUserCriteria criteria);

    IQueryable<UserLevel> SearchUserExcludeFunction(SearchUserExcludeFunctionCriteria criteria);

    IEnumerable<UserPolicy> GetUserPolicy(ICriteria criteria);

    ICommandResult Insert(InsertCustCriteria criteria, ICriteria logCriteria);

    ICommandResult Update(ICriteria criteria, ICriteria logCriteria);

    Hashtable GetUserListSiteFunctionKeyPolicy(ICriteria criteria);
}

public class UserRepository : BaseRepository, IUserRepository
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
        return dynamicRepository.Fetch<User>(criteria).AsQueryable();
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
    

    public IQueryable<UserLevel> Search(SearchUserCriteria criteria)
    {
        return dynamicRepository.Fetch<UserLevel>(criteria).AsQueryable();
    }

    public IQueryable<UserLevel> GetAllByUsernameGroupSite(GetAllByUsernameGroupSiteUserCriteria criteria)
    {
        return dynamicRepository.Fetch<UserLevel>(criteria).AsQueryable();
    }

    public IQueryable<UserLevel> SearchUserExcludeFunction(SearchUserExcludeFunctionCriteria criteria)
    {
        return dynamicRepository.Fetch<UserLevel>(criteria).AsQueryable();
    }

    public IEnumerable<UserPolicy> GetUserPolicy(ICriteria criteria)
    {
        return dynamicRepository.Fetch<UserPolicy>(criteria).AsQueryable();
       
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
}
