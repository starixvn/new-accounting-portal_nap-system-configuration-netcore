
namespace NapAdminCore.Service;

using System.Collections.Generic;
using System.Linq;
using Command;
using Criteria.Function;
using Criteria.Site;
using Criteria.User;
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ValueObject;
using Repository;

public interface ISiteService 
{
    Site GetById(int id);

    Site GetByName(string name);

    Site GetByKey(string key);

    bool GetStatusById(int id);

    SiteLevel GetUserSiteLevel(int userId, int siteId);

    IEnumerable<SiteLevel> GetSiteLevelByUser(int userId, string env = AdminConstant.Uat, bool? isActive = null);

    IEnumerable<Site> GetAvailableSitesForUser(int userId, string env = AdminConstant.Uat, bool? isActive = null);

    IEnumerable<Site> GetAll(string env = AdminConstant.Uat, bool? isActive = null);

    IList<SelectListItem> GetAllSelectListItem(
        string env = AdminConstant.Uat,
        bool? isActive = null,
        int selectedSiteId = 0);

    IEnumerable<Site> GetAllWithNumOfUsers(string env = AdminConstant.Uat, bool? isActive = null);

    IEnumerable<Site> GetByUserAndFunctionKeyIncludeGroup(
        int userId,
        string functionKey,
        string env = AdminConstant.Uat,
        AdminConstant.Policy policy = AdminConstant.Policy.Read);

    IList<SelectListItem> GetSelectListItemByUserAndFunctionKeyIncludeGroup(
        int userId,
        string functionKey,
        string env = AdminConstant.Uat,
        AdminConstant.Policy policy = AdminConstant.Policy.Read,
        int selectedSiteId = 0);

    ICommandResult Insert(InsertSiteCriteria criteria);

    ICommandResult Update(UpdateSiteCriteria criteria);

    ICommandResult UpdateStatus(UpdateStatusSiteCriteria criteria);

    #region SiteUrl services

    IEnumerable<SiteUrl> GetAllSiteUrl(string env);

    IEnumerable<SiteUrl> GetSiteUrlByUser(int userId, string env);

    IEnumerable<SiteUrl> GetSiteUrlBySiteId(int siteId, string env);

    SiteUrl GetSiteUrlById(int siteUrlId);

    ICommandResult UpdateSiteUrl(UpdateSiteUrlCriteria criteria);

    ICommandResult UpdateDefaultSiteUrl(UpdateDefaultSiteUrlCriteria criteria);

    ICommandResult InsertSiteUrl(InsertSiteUrlCriteria criteria);

    #endregion
}
public class SiteService : ISiteService
{
    #region members

    private readonly ISiteRepository siteRepository;
    private readonly IFunctionRepository functionRepository;
    private readonly IDynamicRepository dynamicRepository;
    private readonly LoggedUserContext loggedUserContext;
    #endregion

    #region ctors

    public SiteService(ISiteRepository siteRepository, IFunctionRepository functionRepository, IDynamicRepository dynamicRepository, LoggedUserContext loggedUserContext)
    {
        this.siteRepository = siteRepository;
        this.functionRepository = functionRepository;
        this.dynamicRepository = dynamicRepository;
        this.loggedUserContext = loggedUserContext;
    }

    #endregion

    #region get

    public Site GetById(int id)
    {
        var criteria = new GetByIdSiteCriteria
        {
            SiteId = id
        };

        return dynamicRepository.Get<Site>(criteria);
    }

    public Site GetByName(string name)
    {
        var criteria = new GetByNameSiteCriteria
        {
            SiteName = name
        };

        return dynamicRepository.Get<Site>(criteria);
    }

    public Site GetByKey(string key)
    {
        var criteria = new GetBySiteKeyCriteria
        {
            SiteKey = key
        };

        return dynamicRepository.Get<Site>(criteria);
    }

    public bool GetStatusById(int id)
    {
        var criteria = new GetByIdSiteCriteria
        {
            SiteId = id
        };

        return dynamicRepository.Get<Site>(criteria)?.IsActive == true;
    }

    public IEnumerable<SiteLevel> GetSiteLevelByUser(int userId, string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetByUserSiteCriteria
        {
            UserId = userId,
            Environment = env,
            IsActive = isActive
        };

        return siteRepository.FetchSiteLevel(criteria).OrderBy(sl => sl.Site.Name);
    }

    public IEnumerable<Site> GetAvailableSitesForUser(int userId, string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetAvailableSitesCriteria
        {
            UserId = userId,
            Environment = env,
            IsActive = isActive
        };

        return dynamicRepository.Fetch<Site>(criteria).OrderBy(site => site.Name);
    }

    public IEnumerable<Site> GetAll(string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetAllSiteCriteria
        {
            Environment = env,
            IsActive = isActive
        };

        return dynamicRepository.Fetch<Site>(criteria).OrderBy(site => site.Name);
    }

    public IList<SelectListItem> GetAllSelectListItem(
        string env = AdminConstant.Uat,
        bool? isActive = null,
        int selectedSiteId = 0)
    {
        var criteria = new GetAllSiteCriteria
        {
            Environment = env,
            IsActive = isActive
        };

        IEnumerable<Site> siteList = dynamicRepository.Fetch<Site>(criteria).OrderBy(s => s.Name);

        return siteList.Select(s => new SelectListItem
        {
            Text = s.Name,
            Value = s.Id.ToString(),
            Selected = s.Id == selectedSiteId
        }).ToList();
    }

    public IEnumerable<Site> GetAllWithNumOfUsers(string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetAllSiteWithNumOfUserCriteria
        {
            Environment = env,
            IsActive = isActive
        };

        return dynamicRepository.Fetch<Site>(criteria).OrderBy(site => site.Name);
    }

    public IEnumerable<Site> GetByUserAndFunctionKeyIncludeGroup(
        int userId,
        string functionKey,
        string env = AdminConstant.Uat,
        AdminConstant.Policy policy = AdminConstant.Policy.Read)
    {
        var criteria = new GetByUserAndFunctionKeyIncludeGroupFunctionCriteria
        {
            UserId = userId,
            FunctionKey = functionKey,
            Environment = env
        };

        var functionPolicyList = functionRepository.FetchFunctionPolicy(criteria).OrderBy(s => s.Site.Name);

        return functionPolicyList.Where(f => f.Policy >= (int)policy)
            .Select(s => new Site
            {
                Id = s.Site.Id,
                Name = s.Site.Name
            }).ToList();
    }

    public IList<SelectListItem> GetSelectListItemByUserAndFunctionKeyIncludeGroup(
        int userId,
        string functionKey,
        string env = AdminConstant.Uat,
        AdminConstant.Policy policy = AdminConstant.Policy.Read,
        int selectedSiteId = 0)
    {
        var criteria = new GetByUserAndFunctionKeyIncludeGroupFunctionCriteria
        {
            UserId = userId,
            FunctionKey = functionKey,
            Environment = env
        };


        IEnumerable<FunctionPolicy> functionPolicyList = functionRepository.FetchFunctionPolicy(criteria).OrderBy(s => s.Site.Name);

        return functionPolicyList.Where(f => f.Policy >= (int)policy)
            .Select(s => new SelectListItem
            {
                Text = s.Site.Name,
                Value = s.Site.Id.ToString(),
                Selected = s.Site.Id == selectedSiteId
            }).ToList();
    }

    #endregion

    #region insert/update

    public ICommandResult Insert(InsertSiteCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return siteRepository.Insert(criteria, logCriteria);
    }

    public ICommandResult Update(UpdateSiteCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return siteRepository.Update(criteria, logCriteria);
    }

    public ICommandResult UpdateStatus(UpdateStatusSiteCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return siteRepository.Update(criteria, logCriteria);
    }

    #endregion

    #region SiteLevel services

    public SiteLevel GetUserSiteLevel(int userId, int siteId)
    {
        var criteria = new GetUserSiteLevelSiteCriteria
        {
            UserId = userId,
            SiteId = siteId
        };

        return siteRepository.FetchSiteLevel(criteria).FirstOrDefault();
    }

    #endregion

    #region SiteUrl services

    public IEnumerable<SiteUrl> GetAllSiteUrl(string env)
    {
        var criteria = new GetAllSiteUrlCriteria
        {
            Environment = env
        };

        return siteRepository.FetchSiteUrl(criteria);
    }

    public IEnumerable<SiteUrl> GetSiteUrlByUser(int userId, string env)
    {
        var criteria = new GetSiteUrlByUserCriteria
        {
            UserId = userId,
            Environment = env
        };

        return siteRepository.FetchSiteUrl(criteria).OrderBy(su => su.Site.Name).ToList();
    }

    public IEnumerable<SiteUrl> GetSiteUrlBySiteId(int siteId, string env)
    {
        var criteria = new GetSiteUrlBySiteIdCriteria
        {
            SiteId = siteId,
            Environment = env
        };


        return siteRepository.FetchSiteUrl(criteria).OrderBy(su => su.Site.Name).ToList();
    }

    public SiteUrl GetSiteUrlById(int siteUrlId)
    {
        var criteria = new GetSiteUrlByIdCriteria
        {
            SiteUrlId = siteUrlId
        };
        
        return siteRepository.GetSiteUrl(criteria);
    }

    public ICommandResult UpdateSiteUrl(UpdateSiteUrlCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return siteRepository.Update(criteria, logCriteria);
    }

    public ICommandResult UpdateDefaultSiteUrl(UpdateDefaultSiteUrlCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return siteRepository.Update(criteria, logCriteria);
    }

    public ICommandResult InsertSiteUrl(InsertSiteUrlCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return siteRepository.InsertSiteUrl(criteria, logCriteria);
    }

    #endregion
}
