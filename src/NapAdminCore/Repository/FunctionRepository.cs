
namespace NapAdminCore.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using Command;
using Criteria.Function;
using Criteria.Misc;
using Domain;
using Fanex.Common.Extensions;
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

    ICommandResult Delete(ICriteria criteria, ICriteria logCriteria);
}

public class FunctionRepository : BaseRepository<IFunction>, IFunctionRepository
{
    public IFunction Get(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return db.QueryEntity<Function, Function, Site, User, User, Function>(
            (f, pf, s, cu, mu) =>
            {
                f.Parent = pf;
                f.Site = s;
                f.Creator = cu;
                f.Modifier = mu;
                return f;
            },
            "ParentId,SiteId,CreatorId,ModifierId",
            criteria);
    }

    public int Count(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return (int)db.ExecuteScalar(criteria);
    }

    public IEnumerable<IFunction> Fetch(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return db.Query<Function, Function, Site, User, User, Function>(
            (f, pf, s, cu, mu) =>
            {
                f.Parent = pf;
                f.Site = s;
                f.Creator = cu;
                f.Modifier = mu;
                return f;
            },
            "ParentId,SiteId,CreatorId,ModifierId",
            criteria).OrderBy(f => f.Name).ToList();
    }

    public IEnumerable<FunctionPolicy> FetchFunctionPolicy(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var reader = db.ExecuteReader(criteria);
        var functionPolicyList = new List<FunctionPolicy>();

        while (reader.Read())
        {
            functionPolicyList.Add(new FunctionPolicy
            {
                Function = new Function
                {
                    Id = reader.ToShort("FunctionId"),
                    Name = reader.ToString("FunctionName"),
                    Key = reader.ToString("FunctionKey"),
                    Description = reader.ToString("Description"),
                    Crud = reader.ToByte("CRUD"),
                    DisplayOrder = reader.ToShort("DisplayOrder"),
                    IsActive = reader.ToBool("IsActive"),
                    Site = new Site
                    {
                        Id = reader.ToByte("SiteId"),
                        Name = reader.ToString("SiteName")
                    },
                    Parent = reader.IsDBNull("ParentId") ? null : new Function
                    {
                        Id = reader.ToShort("ParentId"),
                        Name = reader.ToString("ParentName")
                    }
                },
                Site = new Site
                {
                    Id = reader.ToByte("SiteId"),
                    Name = reader.ToString("SiteName")
                },
                Policy = reader.ToInt("Policy")
            });
        }

        return functionPolicyList.OrderBy(f => f.Function.Name).ToList();
    }

    public IEnumerable<FunctionPolicy> FetchFullFunctionPolicy(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var reader = db.ExecuteReader(criteria);
        var functionPolicyList = new List<FunctionPolicy>();

        while (reader.Read())
        {
            functionPolicyList.Add(new FunctionPolicy
            {
                Function = new Function
                {
                    Id = reader.ToShort("FunctionId"),
                    Name = reader.ToString("FunctionName"),
                    Key = reader.ToString("FunctionKey"),
                    Description = reader.ToString("Description"),
                    Crud = reader.ToByte("CRUD"),
                    DisplayOrder = reader.ToShort("DisplayOrder"),
                    IsActive = reader.ToBool("IsActive"),
                    Site = new Site
                    {
                        Id = reader.ToByte("SiteId"),
                        Name = reader.ToString("SiteName")
                    },
                    Parent = reader.IsDBNull("ParentId") ? null : new Function
                    {
                        Id = reader.ToShort("ParentId"),
                        Name = reader.ToString("ParentName")
                    }
                },
                Site = new Site
                {
                    Id = reader.ToByte("SiteId"),
                    Name = reader.ToString("SiteName")
                },
                Policy = reader.ToInt("Policy"),
                Creator = reader.IsDBNull("CreatorId") ? null : new User
                {
                    Id = reader.ToInt("CreatorId"),
                    Name = reader.ToString("CreatorUsername")
                },
                CreatedDate = reader.ToDateTime("CreatedDate"),
                Modifier = reader.IsDBNull("ModifierId") ? null : new User
                {
                    Id = reader.ToInt("ModifierId"),
                    Name = reader.ToString("ModifierUsername")
                },
                ModifiedDate = reader.ToDateTime("ModifiedDate")
            });
        }

        return functionPolicyList.OrderBy(f => f.Function.Name).ToList();
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

    public ICommandResult Delete(ICriteria criteria, ICriteria logCriteria)
    {
        throw new NotImplementedException();
    }
}
