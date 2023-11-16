
namespace NapAdminCore.Repository;

using System.Collections.Generic;
using System.Linq;
using Fanex.Data.Repository;
using Models;
using Models.ValueObject;

public interface ISiteRepository: IBaseRepository
{
    SiteUrl GetSiteUrl(ICriteria criteria);

    IEnumerable<SiteLevel> FetchSiteLevel(ICriteria criteria);

    IEnumerable<SiteUrl> FetchSiteUrl(ICriteria criteria);
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
}
