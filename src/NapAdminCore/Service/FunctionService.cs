
namespace NapAdminCore.Service;

using System;
using System.Collections.Generic;
using System.Linq;
using Command;
using Criteria.Function;
using Domain;
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ValueObject;
using Repository;

public interface IFunctionService 
{
    #region Gets Function

    IFunction GetById(int id);

    IFunction GetByNameAndSite(string name, int siteId);

    IFunction GetByKeyAndSite(string key, int siteId);

    #endregion

    #region Gets Functions

    IEnumerable<IFunction> GetAll(string env = AdminConstant.Uat, bool? isActive = null);

    Dictionary<ISite, IEnumerable<IFunction>> GetAllAndGroupBySite(string env = AdminConstant.Uat, bool? isActive = null);

    IEnumerable<IFunction> GetBySite(int siteId, string env = AdminConstant.Uat, bool? isActive = null);

    IList<SelectListItem> GetSelectListItemBySite(
        int siteId,
        string env = AdminConstant.Uat,
        bool? isActive = null,
        int selectedFunctionId = 0);

    IEnumerable<IFunction> GetBySiteInHierarchy(int siteId, string env = AdminConstant.Uat, bool? isActive = null);

    IEnumerable<IFunction> GetParentById(int id);

    IEnumerable<IFunction> Search(string functionName, int siteId, string env = AdminConstant.Uat, bool? isActive = null);

    IEnumerable<IFunction> GetByParentKeyAndSite(GetByParentKeyAndSiteCriteria criteria);

    #endregion

    #region Gets Functions Policy

    IEnumerable<FunctionPolicy> GetByUser(int userId, string env = AdminConstant.Uat);

    Dictionary<ISite, IEnumerable<FunctionPolicy>> GetByUserAndGroupBySite(int userId, string env = AdminConstant.Uat);

    IEnumerable<FunctionPolicy> GetByGroup(int groupId, string env = AdminConstant.Uat);

    Dictionary<ISite, IEnumerable<FunctionPolicy>> GetByGroupAndGroupBySite(int groupId, string env = AdminConstant.Uat);

    IEnumerable<FunctionPolicy> GetByGroupAndSite(int groupId, int siteId, string env = AdminConstant.Uat);

    IEnumerable<FunctionPolicy> GetByUserAndSite(int userId, int siteId, string env = AdminConstant.Uat);

    IEnumerable<FunctionPolicy> GetByUserAndSiteIncludeGroup(int userId, int siteId, string env = AdminConstant.Uat);

    IEnumerable<FunctionPolicy> GetByUserAndFunctionKeyIncludeGroup(
        int userId,
        string functionKey,
        string env = AdminConstant.Uat);

    #endregion

    bool CheckUserFunctionPolicy(int userId, int functionId, int policy);

    ICommandResult Duplicate(DuplicateFunctionCriteria criteria);

    ICommandResult Insert(InsertFunctionCriteria criteria);

    ICommandResult Update(UpdateFunctionCriteria criteria);

    ICommandResult UpdateStatus(UpdateStatusFunctionCriteria criteria);

    ICommandResult AssignFunctionToUsers(AssignFunctionToUsersCriteria criteria);

    ICommandResult AssignFunctionToGroups(AssignFunctionToGroupsCriteria criteria);

    ICommandResult CopyPermission(CopyPermissionCriteria criteria);
}
public class FunctionService : IFunctionService
{
    private readonly IFunctionRepository functionRepository;
    private readonly IDynamicRepository dynamicRepository;
    private readonly LoggedUserContext loggedUserContext;
    #region Ctors

    public FunctionService(IFunctionRepository functionRepository, IDynamicRepository dynamicRepository, LoggedUserContext loggedUserContext)
    {
        this.functionRepository = functionRepository;
        this.dynamicRepository = dynamicRepository;
        this.loggedUserContext = loggedUserContext;
    }

    #endregion

    #region Gets Function

    public IFunction GetById(int id)
    {
        var criteria = new GetByIdFunctionCriteria
        {
            FunctionId = id
        };

        return dynamicRepository.Get<IFunction>(criteria);
    }

    public IFunction GetByNameAndSite(string name, int siteId)
    {
        var criteria = new GetByNameAndSiteFunctionCriteria
        {
            FunctionName = name,
            SiteId = siteId
        };

        return dynamicRepository.Get<IFunction>(criteria);
    }

    public IFunction GetByKeyAndSite(string key, int siteId)
    {
        var criteria = new GetByKeyAndSiteFunctionCriteria
        {
            FunctionKey = key,
            SiteId = siteId
        };

        return dynamicRepository.Get<IFunction>(criteria);
    }

    #endregion

    #region Get Functions

    public IEnumerable<IFunction> GetAll(string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetAllFunctionCriteria
        {
            Environment = env,
            IsActive = isActive
        };

        return dynamicRepository.Fetch<IFunction>(criteria);
    }

    public Dictionary<ISite, IEnumerable<IFunction>> GetAllAndGroupBySite(
        string env = AdminConstant.Uat,
        bool? isActive = null)
    {
        var criteria = new GetAllFunctionCriteria
        {
            Environment = env,
            IsActive = isActive
        };

        var functionList = dynamicRepository.Fetch<IFunction>(criteria);

        var result = new Dictionary<ISite, IEnumerable<IFunction>>();

        var siteList = functionList.GroupBy(f => f.Site.Id)
            .Select(s => new Site
            {
                Id = s.Key,
                Name = s.FirstOrDefault(f => f.Site.Id == s.Key)?.Site.Name
            }).ToList();

        foreach (var site in siteList)
        {
            var subFunctionList = functionList.Where(f => f.Site.Id == site.Id).ToList();

            result.Add(site, subFunctionList);
        }

        return result;
    }

    public IEnumerable<IFunction> GetBySite(int siteId, string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetBySiteFunctionCriteria
        {
            SiteId = siteId,
            Environment = env,
            IsActive = isActive
        };

        return dynamicRepository.Fetch<IFunction>(criteria);
    }

    public IList<SelectListItem> GetSelectListItemBySite(
        int siteId,
        string env = AdminConstant.Uat,
        bool? isActive = null,
        int selectedFunctionId = 0)
    {
        var criteria = new GetBySiteFunctionCriteria
        {
            SiteId = siteId,
            Environment = env,
            IsActive = isActive
        };


        var functionList = dynamicRepository.Fetch<IFunction>(criteria);

        return functionList.Select(s => new SelectListItem
        {
            Text = s.Name,
            Value = s.Id.ToString(),
            Selected = s.Id == selectedFunctionId
        }).ToList();
    }

    public IEnumerable<IFunction> GetBySiteInHierarchy(int siteId, string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetBySiteFunctionCriteria
        {
            SiteId = siteId,
            Environment = env,
            IsActive = isActive
        };

        var functionList = dynamicRepository.Fetch<IFunction>(criteria).OrderBy(x => x.DisplayOrder);

        return SortFunctionsInHierarchy(functionList);
    }

    public IEnumerable<IFunction> GetParentById(int id)
    {
        var criteria = new GetParentByIdFunctionCriteria
        {
            FunctionId = id
        };

        return dynamicRepository.Fetch<IFunction>(criteria);
    }

    public IEnumerable<IFunction> Search(
        string functionName,
        int siteId,
        string env = AdminConstant.Uat,
        bool? isActive = null)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IFunction> GetByParentKeyAndSite(GetByParentKeyAndSiteCriteria criteria)
    {
        return dynamicRepository.Fetch<IFunction>(criteria);
    }

    #endregion

    #region Gets Function Policy

    public IEnumerable<FunctionPolicy> GetByUser(int userId, string env = AdminConstant.Uat)
    {
        var criteria = new GetByUserFunctionCriteria
        {
            UserId = userId,
            Environment = env
        };

        return functionRepository.FetchFullFunctionPolicy(criteria);
    }

    public Dictionary<ISite, IEnumerable<FunctionPolicy>> GetByUserAndGroupBySite(int userId, string env = AdminConstant.Uat)
    {
        var criteria = new GetByUserFunctionCriteria
        {
            UserId = userId,
            Environment = env
        };

        var functionPolicyList = functionRepository.FetchFullFunctionPolicy(criteria);

        var result = new Dictionary<ISite, IEnumerable<FunctionPolicy>>();

        var siteList = functionPolicyList.GroupBy(f => f.Site.Id)
            .Select(s => new Site
            {
                Id = s.Key,
                Name = s.FirstOrDefault(f => f.Site.Id == s.Key)?.Site.Name
            }).ToList();

        foreach (var site in siteList)
        {
            var subFunctionPolicyList = functionPolicyList.Where(f => f.Site.Id == site.Id).ToList();

            result.Add(site, subFunctionPolicyList);
        }

        return result;
    }

    public IEnumerable<FunctionPolicy> GetByGroup(int groupId, string env = AdminConstant.Uat)
    {
        var criteria = new GetByGroupFunctionCriteria
        {
            GroupId = groupId,
            Environment = env
        };

        return functionRepository.FetchFullFunctionPolicy(criteria);
    }

    public IEnumerable<FunctionPolicy> GetByGroupAndSite(int groupId, int siteId, string env = AdminConstant.Uat)
    {
        var criteria = new GetByGroupAndSiteFunctionCriteria
        {
            GroupId = groupId,
            SiteId = siteId,
            Environment = env
        };

        return functionRepository.FetchFullFunctionPolicy(criteria);
    }

    public Dictionary<ISite, IEnumerable<FunctionPolicy>> GetByGroupAndGroupBySite(int groupId, string env = AdminConstant.Uat)
    {
        var criteria = new GetByGroupFunctionCriteria
        {
            GroupId = groupId,
            Environment = env
        };

        var functionPolicyList = functionRepository.FetchFullFunctionPolicy(criteria);

        var result = new Dictionary<ISite, IEnumerable<FunctionPolicy>>();

        var siteList = functionPolicyList.GroupBy(f => f.Site.Id)
            .Select(s => new Site
            {
                Id = s.Key,
                Name = s.FirstOrDefault(f => f.Site.Id == s.Key)?.Site.Name
            }).ToList();

        foreach (var site in siteList)
        {
            var subFunctionPolicyList = functionPolicyList.Where(f => f.Site.Id == site.Id).ToList();

            result.Add(site, subFunctionPolicyList);
        }

        return result;
    }

    public IEnumerable<FunctionPolicy> GetByUserAndSite(int userId, int siteId, string env = AdminConstant.Uat)
    {
        var criteria = new GetByUserAndSiteFunctionCriteria
        {
            UserId = userId,
            SiteId = siteId,
            Environment = env
        };

        return functionRepository.FetchFullFunctionPolicy(criteria);
    }

    public IEnumerable<FunctionPolicy> GetByUserAndSiteIncludeGroup(
        int userId,
        int siteId,
        string env = AdminConstant.Uat)
    {
        var criteria = new GetByUserAndSiteIncludeGroupFunctionCriteria
        {
            UserId = userId,
            SiteId = siteId,
            Environment = env
        };

        return functionRepository.FetchFunctionPolicy(criteria);
    }

    public IEnumerable<FunctionPolicy> GetByUserAndFunctionKeyIncludeGroup(
        int userId,
        string functionKey,
        string env = AdminConstant.Uat)
    {
        var criteria = new GetByUserAndFunctionKeyIncludeGroupFunctionCriteria
        {
            UserId = userId,
            FunctionKey = functionKey,
            Environment = env
        };

        return functionRepository.FetchFunctionPolicy(criteria);
    }

    #endregion

    public bool CheckUserFunctionPolicy(int userId, int functionId, int policy)
    {
        throw new NotImplementedException();
    }

    public ICommandResult Duplicate(DuplicateFunctionCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();
        return functionRepository.Duplicate(criteria, logCriteria);
    }

    public ICommandResult Insert(InsertFunctionCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();
        return functionRepository.Insert(criteria, logCriteria);
    }

    public ICommandResult Update(UpdateFunctionCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();
        return functionRepository.Update(criteria, logCriteria);
    }

    public ICommandResult UpdateStatus(UpdateStatusFunctionCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();
        return functionRepository.Update(criteria, logCriteria);
    }

    public ICommandResult AssignFunctionToUsers(AssignFunctionToUsersCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();
        return functionRepository.Update(criteria, logCriteria);
    }

    public ICommandResult AssignFunctionToGroups(AssignFunctionToGroupsCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return functionRepository.Update(criteria, logCriteria);
    }

    private IEnumerable<IFunction> SortFunctionsInHierarchy(IEnumerable<IFunction> functionList, int parentId = 0)
    {
        var sortedFunctionList = new List<IFunction>();

        foreach (var function in functionList)
        {
            if (function.Parent?.Id == parentId)
            {
                sortedFunctionList.Add(function);
                sortedFunctionList.AddRange(SortFunctionsInHierarchy(functionList, function.Id));
            }
        }

        return sortedFunctionList;
    }

    public ICommandResult CopyPermission(CopyPermissionCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return functionRepository.CopyPermission(criteria, logCriteria);
    }
}
