
namespace NapAdminCore.Service;

using System.Collections.Generic;
using System.Linq;
using Criteria.Function;
using Criteria.Site;
using Criteria.User;
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ValueObject;
using NapAdminCore.Criteria;
using Repository;

public interface ISiteService 
{
    Site GetById(int id);

    Site GetByName(string name);

    Site GetByKey(string key);

    IList<SiteLevel> GetSiteLevelByUser(int userId, string env = AdminConstant.Uat, bool? isActive = null);

    IList<Site> GetAvailableSitesForUser(int userId, string env = AdminConstant.Uat, bool? isActive = null);

    IList<Site> GetAll(string env = AdminConstant.Uat, bool? isActive = null);

    IList<SelectListItem> GetAllSelectListItem(
        string env = AdminConstant.Uat,
        bool? isActive = null,
        int selectedSiteId = 0);

    IList<Site> GetAllWithNumOfUsers(string env = AdminConstant.Uat, bool? isActive = null);

    IList<Site> GetByUserAndFunctionKeyIncludeGroup(
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

    CommandResult Insert(InsertSiteCommand criteria);

    CommandResult Update(UpdateSiteCommand criteria);

    CommandResult UpdateStatus(UpdateStatusSiteCommand criteria);

    #region SiteUrl services

    IList<SiteUrl> GetAllSiteUrl(string env);

    IList<SiteUrl> GetSiteUrlBySiteId(int siteId, string env);

    SiteUrl GetSiteUrlById(int siteUrlId);

    CommandResult UpdateSiteUrl(UpdateSiteUrlCommand criteria);

    CommandResult UpdateDefaultSiteUrl(UpdateDefaultSiteUrlCommand criteria);

    CommandResult InsertSiteUrl(InsertSiteUrlCommand criteria);

    #endregion
}
public class SiteService(ISimpleRepository repository, IDynamicRepository dynamicRepository, LoggedUserContext loggedUserContext)
    : ISiteService
{
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
    
    public IList<SiteLevel> GetSiteLevelByUser(int userId, string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetByUserSiteCriteria
        {
            UserId = userId,
            Environment = env,
            IsActive = isActive
        };

        return dynamicRepository.Fetch<SiteLevel>(criteria).OrderBy(sl => sl.SiteName).ToList();
    }

    public IList<Site> GetAvailableSitesForUser(int userId, string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetAvailableSitesCriteria
        {
            UserId = userId,
            Environment = env,
            IsActive = isActive
        };

        return dynamicRepository.Fetch<Site>(criteria).OrderBy(site => site.Name).ToList();
    }

    public IList<Site> GetAll(string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetAllSiteCriteria
        {
            Environment = env,
            IsActive = isActive
        };

        return dynamicRepository.Fetch<Site>(criteria).OrderBy(site => site.Name).ToList();
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

        var siteList = dynamicRepository.Fetch<Site>(criteria).OrderBy(s => s.Name).ToList();

        return siteList.Select(s => new SelectListItem
        {
            Text = s.Name,
            Value = s.Id.ToString(),
            Selected = s.Id == selectedSiteId
        }).ToList();
    }

    public IList<Site> GetAllWithNumOfUsers(string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetAllSiteWithNumOfUserCriteria
        {
            Environment = env,
            IsActive = isActive
        };

        return dynamicRepository.Fetch<Site>(criteria).OrderBy(site => site.Name).ToList();
    }

    public IList<Site> GetByUserAndFunctionKeyIncludeGroup(
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

        var functionPolicyList = repository.FetchFunctionPolicy(criteria)
            .OrderBy(s => s.SiteName)
            .ToList();

        return functionPolicyList.Where(f => f.Policy >= (int)policy)
            .Select(s => new Site
            {
                Id = s.SiteId,
                Name = s.SiteName
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


        var functionPolicyList = repository.FetchFunctionPolicy(criteria).OrderBy(s => s.SiteName).ToList();

        return functionPolicyList.Where(f => f.Policy >= (int)policy)
            .Select(s => new SelectListItem
            {
                Text = s.SiteName,
                Value = s.SiteId.ToString(),
                Selected = s.SiteId == selectedSiteId
            }).ToList();
    }

    #endregion

    public CommandResult Insert(InsertSiteCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return repository.Execute(criteria, logCriteria);
    }

    public CommandResult Update(UpdateSiteCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return repository.Execute(criteria, logCriteria);
    }

    public CommandResult UpdateStatus(UpdateStatusSiteCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return repository.Execute(criteria, logCriteria);
    }

    #region SiteUrl services

    public IList<SiteUrl> GetAllSiteUrl(string env)
    {
        var criteria = new GetAllSiteUrlCriteria
        {
            Environment = env
        };

        return dynamicRepository.Fetch<SiteUrl>(criteria).ToList();
    }

    public IList<SiteUrl> GetSiteUrlBySiteId(int siteId, string env)
    {
        var criteria = new GetSiteUrlBySiteIdCriteria
        {
            SiteId = siteId,
            Environment = env
        };


        return dynamicRepository.Fetch<SiteUrl>(criteria).OrderBy(su => su.SiteName).ToList();
    }

    public SiteUrl GetSiteUrlById(int siteUrlId)
    {
        var criteria = new GetSiteUrlByIdCriteria
        {
            SiteUrlId = siteUrlId
        };
        
        return dynamicRepository.Get<SiteUrl>(criteria);
    }

    public CommandResult UpdateSiteUrl(UpdateSiteUrlCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return repository.Execute(criteria, logCriteria);
    }

    public CommandResult UpdateDefaultSiteUrl(UpdateDefaultSiteUrlCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return repository.Execute(criteria, logCriteria);
    }

    public CommandResult InsertSiteUrl(InsertSiteUrlCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return repository.Execute(criteria, logCriteria);
    }

    #endregion
}
