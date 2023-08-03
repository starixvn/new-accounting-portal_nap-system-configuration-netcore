
namespace NapAdminCore.Repository;

using System;
using System.Linq;
using Command;
using Criteria.Trace;
using Fanex.Data;
using Fanex.Data.Repository;
using Helpers;
using Models;
using Models.ValueObject;
public interface ITraceRepository : IDynamicRepository
{
    IQueryable<LogExecSp> SearchLogExecSp(GetAllLogCriteria criteria);

    IQueryable<LogUserLogin> SearchLogUserLogin(GetAllLogUserLoginCriteria criteria);

    IQueryable<LogFunction> SearchLogFunction(GetAllLogFunctionCriteria criteria);

    LogExecSp GetById(ICriteria criteria);

    ICommandResult InsertLog(ICriteria criteria);
}
public class TraceRepository : DynamicRepository, ITraceRepository
{
    public IQueryable<LogExecSp> SearchLogExecSp(GetAllLogCriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var hashtable = HashtableExtensions.ToHashtable(criteria);

        var logList = db.Query<LogExecSp>(hashtable);

        criteria.TotalRows = Convert.ToInt32(hashtable["totalrows"]);

        return logList.AsQueryable();
    }

    public LogExecSp GetById(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var hashtable = HashtableExtensions.ToHashtable(criteria);

        return db.QueryEntity<LogExecSp>(hashtable);
    }

    public ICommandResult InsertLog(ICriteria criteria)
    {
        var hashtable = HashtableExtensions.ToHashtable(criteria);

        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        db.ExecuteNonQuery(hashtable);

        return new CommandResult();
    }

    public IQueryable<LogUserLogin> SearchLogUserLogin(GetAllLogUserLoginCriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var hashtable = HashtableExtensions.ToHashtable(criteria);

        var logList = db.Query<LogUserLogin, User, Site, LogUserLogin>(
            (l, u, s) => { l.User = u; l.Site = s; return l; },
            "UserId,SiteId",
            hashtable);

        criteria.TotalRows = Convert.ToInt32(hashtable["totalrows"]);

        return logList.AsQueryable();
    }

    public IQueryable<LogFunction> SearchLogFunction(GetAllLogFunctionCriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var hashtable = HashtableExtensions.ToHashtable(criteria);

        var logList = db.Query<LogFunction, Function, User, Site, LogFunction>(
            (l, f, u, s) => { l.Function = f; l.User = u; l.Site = s; return l; },
            "FunctionId,UserId,SiteId",
            hashtable);

        criteria.TotalRows = Convert.ToInt32(hashtable["totalrows"]);

        return logList.AsQueryable();
    }
}
