
namespace NapAdminCore.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using Command;
using Criteria.Function;
using Criteria.Misc;
using Fanex.Data;
using Fanex.Data.Repository;
using Models;
using Models.ValueObject;
using Helpers;

public interface IFunctionRepository
{
    IEnumerable<FunctionPolicy> FetchFunctionPolicy(ICriteria criteria);

    IEnumerable<FunctionPolicy> FetchFullFunctionPolicy(ICriteria criteria);

    ICommandResult Duplicate(DuplicateFunctionCriteria criteria, ICriteria logCriteria);

    ICommandResult Insert(InsertFunctionCriteria criteria, ICriteria logCriteria);

    ICommandResult Update(ICriteria criteria, ICriteria logCriteria);

    ICommandResult CopyPermission(CopyPermissionCriteria criteria, ICriteria logCriteria);
}

public class FunctionRepository : BaseRepository, IFunctionRepository
{
    private readonly IDynamicRepository dynamicRepository;

    public FunctionRepository(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public Function Get(ICriteria criteria)
    {
        return dynamicRepository.Get<Function>(criteria);
    }

    public int Count(ICriteria criteria)
    {
        return dynamicRepository.Get<int>(criteria);
    }

    public IEnumerable<Function> Fetch(ICriteria criteria)
    {
        return dynamicRepository.Fetch<Function>(criteria);
    }

    public IEnumerable<FunctionPolicy> FetchFunctionPolicy(ICriteria criteria)
    {
        var policies = dynamicRepository.Fetch<FunctionPolicy>(criteria);
        return policies.OrderBy(f => f.Name).ToList();
    }

    public IEnumerable<FunctionPolicy> FetchFullFunctionPolicy(ICriteria criteria)
    {
        var policies = dynamicRepository.Fetch<FunctionPolicy>(criteria);
        return policies.OrderBy(f => f.Name).ToList();
    }

    public ICommandResult Duplicate(DuplicateFunctionCriteria criteria, ICriteria logCriteria)
    {
        var hashtable = HashtableExtensions.ToHashtable(criteria, logCriteria, new ResultCriteria());
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        db.ExecuteNonQuery(hashtable);

        criteria.FunctionId = Convert.ToInt32(db.GetParameterValue("FunctionId"));

        return GetCommandResult(hashtable);
    }

    public ICommandResult Insert(InsertFunctionCriteria criteria, ICriteria logCriteria)
    {
        var hashtable = HashtableExtensions.ToHashtable(criteria, logCriteria, new ResultCriteria());
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        db.ExecuteNonQuery(hashtable);

        criteria.FunctionId = Convert.ToInt32(db.GetParameterValue("FunctionId"));

        return GetCommandResult(hashtable);
    }

    public ICommandResult Update(ICriteria criteria, ICriteria logCriteria)
    {
        var hashtable = HashtableExtensions.ToHashtable(criteria, logCriteria, new ResultCriteria());

        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        db.ExecuteNonQuery(hashtable);

        return GetCommandResult(hashtable);
    }

    public ICommandResult CopyPermission(CopyPermissionCriteria criteria, ICriteria logCriteria)
    {
        var hashtable = HashtableExtensions.ToHashtable(criteria, logCriteria, new ResultCriteria());

        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());

        db.ExecuteNonQuery(hashtable);

        return GetCommandResult(hashtable);

    }
}
