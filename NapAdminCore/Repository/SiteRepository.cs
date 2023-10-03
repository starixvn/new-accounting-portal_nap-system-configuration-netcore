
namespace NapAdminCore.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using Command;
using Criteria.Misc;
using Criteria.Site;
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
}
public class SiteRepository : BaseRepository, ISiteRepository
{
    private readonly IDynamicRepository dynamicRepository;
    public SiteRepository(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }
    
    public SiteUrl GetSiteUrl(ICriteria criteria)
    {
        return dynamicRepository.Get<SiteUrl>(criteria);
    }

    public int Count(ICriteria criteria)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Site> Fetch(ICriteria criteria)
    {
        return dynamicRepository.Fetch<Site>(criteria).OrderBy(s => s.Name).ToList();
    }

    public IEnumerable<SiteLevel> FetchSiteLevel(ICriteria criteria)
    {
        return dynamicRepository.Fetch<SiteLevel>(criteria).OrderBy(s => s.SiteName).ToList();
    }

    public IEnumerable<SiteUrl> FetchSiteUrl(ICriteria criteria)
    {
        return dynamicRepository.Fetch<SiteUrl>(criteria).OrderBy(s => s.SiteName).ToList();
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
}
