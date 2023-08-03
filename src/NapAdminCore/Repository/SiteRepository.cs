
namespace NapAdminCore.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using Command;
using Criteria.Misc;
using Criteria.Site;
using Domain;
using Fanex.Data;
using Fanex.Data.Repository;
using Models;
using Models.ValueObject;
using Helpers;

public interface ISiteRepository 
{
    SiteUrl GetSiteUrl(ICriteria criteria);

    IEnumerable<SiteLevel> FetchSiteLevel(ICriteria criteria);

    IEnumerable<SiteUrl> FetchSiteUrl(ICriteria criteria);

    ICommandResult Insert(InsertSiteCriteria criteria, ICriteria logCriteria);

    ICommandResult InsertSiteUrl(InsertSiteUrlCriteria criteria, ICriteria logCriteria);

    ICommandResult Update(ICriteria criteria, ICriteria logCriteria);

    ICommandResult Delete(ICriteria criteria, ICriteria logCriteria);
}
public class SiteRepository : BaseRepository<ISite>, ISiteRepository
{
    public  ISite Get(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return db.QueryEntity<Site>(criteria);
    }

    public SiteUrl GetSiteUrl(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return db.QueryEntity<SiteUrl, Site, SiteUrl>(
            (su, s) =>
            {
                su.Site = s;
                return su;
            },
            "Id",
            criteria);
    }

    public  int Count(ICriteria criteria)
    {
        throw new NotImplementedException();
    }

    public  IEnumerable<ISite> Fetch(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return db.Query<Site>(criteria).OrderBy(s => s.Name).ToList();
    }

    public IEnumerable<SiteLevel> FetchSiteLevel(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return db.Query<SiteLevel, Site, User, User, SiteLevel>(
            (sl, s, cu, mu) =>
            {
                sl.Site = s;
                sl.Creator = cu;
                sl.Modifier = mu;
                return sl;
            },
            "SiteId,CreatorId,ModifierId",
            criteria).OrderBy(s => s.Site.Name).ToList();
    }

    public IEnumerable<SiteUrl> FetchSiteUrl(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return db.Query<SiteUrl, Site, SiteUrl>(
            (su, s) =>
            {
                su.Site = s;
                return su;
            },
            "Id",
            criteria).OrderBy(s => s.Site.Name).ToList();
    }

    public ICommandResult Insert(InsertSiteCriteria criteria, ICriteria logCriteria)
    {
        var hashtable = HashtableExtensions.ToHashtable(criteria, logCriteria, new ResultCriteria());

        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        db.ExecuteNonQuery(hashtable);

        criteria.SiteId = Convert.ToInt32(db.GetParameterValue("SiteId"));

        return GetCommandResult(hashtable);
    }

    public ICommandResult InsertSiteUrl(InsertSiteUrlCriteria criteria, ICriteria logCriteria)
    {
        var hashtable = HashtableExtensions.ToHashtable(criteria, logCriteria, new ResultCriteria());

        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        db.ExecuteNonQuery(hashtable);

        criteria.SiteUrlId = Convert.ToInt32(db.GetParameterValue("SiteUrlId"));

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
