
namespace NapAdminCore.Service;

using System.Collections.Generic;
using System.Linq;
using Criteria.Function;
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ValueObject;
using Criteria;
using NapApiCaching;
using Repository;
using Function = Models.Function;

public interface IFunctionService
{
    #region Gets Function

    Function GetById(int id);

    Function GetByNameAndSite(string name, int siteId);

    Function GetByKeyAndSite(string key, int siteId);

    #endregion

    #region Gets Functions

    IEnumerable<Function> GetBySite(int siteId, string env = AdminConstant.Uat, bool? isActive = null);

    IList<SelectListItem> GetSelectListItemBySite(
        int siteId,
        string env = AdminConstant.Uat,
        bool? isActive = null,
        int selectedFunctionId = 0);

    IEnumerable<Function> GetBySiteInHierarchy(int siteId, string env = AdminConstant.Uat, bool? isActive = null);

    #endregion

    #region Gets Functions Policy

    IEnumerable<FunctionPolicy> GetByUser(int userId, string env = AdminConstant.Uat);

    Dictionary<Site, IEnumerable<FunctionPolicy>> GetByUserAndGroupBySite(int userId, string env = AdminConstant.Uat);

    IEnumerable<FunctionPolicy> GetByGroupAndSite(int groupId, int siteId, string env = AdminConstant.Uat);

    IEnumerable<FunctionPolicy> GetByUserAndSite(int userId, int siteId, string env = AdminConstant.Uat);

    IEnumerable<FunctionPolicy> GetByUserAndSiteIncludeGroup(int userId, int siteId, string env = AdminConstant.Uat);

    #endregion

    CommandResult Duplicate(DuplicateFunctionCommand criteria);

    CommandResult Insert(InsertFunctionCommand criteria);

    CommandResult Update(UpdateFunctionCommand criteria);

    CommandResult UpdateStatus(UpdateStatusFunctionCommand criteria);

    CommandResult AssignFunctionToUsers(AssignFunctionToUsersCommand criteria);

    CommandResult AssignFunctionToGroups(AssignFunctionToGroupsCommand criteria);

    CommandResult CopyPermission(CopyPermissionCommand criteria);
}
public class FunctionService(UpdateRepository repository, IDynamicRepository dynamicRepository,
        LoggedUserContext loggedUserContext)
    : IFunctionService
{

    #region Gets Function

    public Function GetById(int id)
    {
        var criteria = new GetByIdFunctionCriteria
        {
            FunctionId = id
        };

        return dynamicRepository.Get<Function>(criteria);
    }

    public Function GetByNameAndSite(string name, int siteId)
    {
        var criteria = new GetByNameAndSiteFunctionCriteria
        {
            FunctionName = name,
            SiteId = siteId
        };

        return dynamicRepository.Get<Function>(criteria);
    }

    public Function GetByKeyAndSite(string key, int siteId)
    {
        var criteria = new GetByKeyAndSiteFunctionCriteria
        {
            FunctionKey = key,
            SiteId = siteId
        };

        return dynamicRepository.Get<Function>(criteria);
    }

    #endregion

    #region Get Functions

    public IEnumerable<Function> GetBySite(int siteId, string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetBySiteFunctionCriteria
        {
            SiteId = siteId,
            Environment = env,
            IsActive = isActive
        };

        return dynamicRepository.Fetch<Function>(criteria);
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


        var functionList = dynamicRepository.Fetch<Function>(criteria);

        return functionList.Select(s => new SelectListItem
        {
            Text = s.Name,
            Value = s.Id.ToString(),
            Selected = s.Id == selectedFunctionId
        })
            .OrderBy(f => f.Text)
            .ToList();
    }

    public IEnumerable<Function> GetBySiteInHierarchy(int siteId, string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetBySiteFunctionCriteria
        {
            SiteId = siteId,
            Environment = env,
            IsActive = isActive
        };

        var functionList = dynamicRepository.Fetch<Function>(criteria).OrderBy(x => x.DisplayOrder);

        return SortFunctionsInHierarchy(functionList);
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

        var policies = dynamicRepository.Fetch<FunctionPolicy>(criteria);
        return policies.OrderBy(f => f.Name).ToList();
    }

    public Dictionary<Site, IEnumerable<FunctionPolicy>> GetByUserAndGroupBySite(int userId, string env = AdminConstant.Uat)
    {
        var criteria = new GetByUserFunctionCriteria
        {
            UserId = userId,
            Environment = env
        };

        var policies = dynamicRepository.Fetch<FunctionPolicy>(criteria);
        var functionPolicyList= policies.OrderBy(f => f.Name).ToList();

        var result = new Dictionary<Site, IEnumerable<FunctionPolicy>>();

        var siteList = functionPolicyList.GroupBy(f => f.SiteId)
            .Select(s => new Site
            {
                Id = s.Key,
                Name = s.FirstOrDefault(f => f.SiteId == s.Key)?.SiteName
            }).ToList();

        foreach (var site in siteList)
        {
            var subFunctionPolicyList = functionPolicyList.Where(f => f.SiteId == site.Id).ToList();

            result.Add(site, subFunctionPolicyList);
        }

        return result;
    }
    
    public IEnumerable<FunctionPolicy> GetByGroupAndSite(int groupId, int siteId, string env = AdminConstant.Uat)
    {
        var criteria = new GetByGroupAndSiteFunctionCriteria
        {
            GroupId = groupId,
            SiteId = siteId,
            Environment = env
        };

        var policies = dynamicRepository.Fetch<FunctionPolicy>(criteria);
        return policies.OrderBy(f => f.Name).ToList();
    }
    
    public IEnumerable<FunctionPolicy> GetByUserAndSite(int userId, int siteId, string env = AdminConstant.Uat)
    {
        var criteria = new GetByUserAndSiteFunctionCriteria
        {
            UserId = userId,
            SiteId = siteId,
            Environment = env
        };

        var policies = dynamicRepository.Fetch<FunctionPolicy>(criteria);
        return policies.OrderBy(f => f.Name).ToList();
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

        var policies = dynamicRepository.Fetch<FunctionPolicy>(criteria);
        return policies.OrderBy(f => f.Name).ToList();
    }

    #endregion

    public CommandResult Duplicate(DuplicateFunctionCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();
        var result= repository.Execute(criteria, logCriteria);

        DataCachingJobCommands.Instance.AddFunctionCommand();

        return result;
    }

    public CommandResult Insert(InsertFunctionCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();
        var result = repository.Execute(criteria, logCriteria);

        DataCachingJobCommands.Instance.AddFunctionCommand();
        return result;
    }

    public CommandResult Update(UpdateFunctionCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();
        var result = repository.Execute(criteria, logCriteria);

        DataCachingJobCommands.Instance.AddFunctionCommand();
        DataCachingJobCommands.Instance.AddUserFunctionPolicyCommand(criteria.SiteId);

        return result;
    }

    public CommandResult UpdateStatus(UpdateStatusFunctionCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();
        var result = repository.Execute(criteria, logCriteria);
        
        var oldFunction = GetById(criteria.FunctionId);
        DataCachingJobCommands.Instance.AddFunctionCommand();
        DataCachingJobCommands.Instance.AddUserFunctionPolicyCommand(oldFunction.SiteId);
        
        return result;
    }

    public CommandResult AssignFunctionToUsers(AssignFunctionToUsersCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();
        var result= repository.Execute(criteria, logCriteria);
        
        DataCachingJobCommands.Instance.AddUserFunctionPolicyCommand(criteria.SiteId);

        return result;
    }

    public CommandResult AssignFunctionToGroups(AssignFunctionToGroupsCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        var  result= repository.Execute(criteria, logCriteria);

        DataCachingJobCommands.Instance.AddUserFunctionPolicyCommand(criteria.SiteId);

        return result;
    }

    private IEnumerable<Function> SortFunctionsInHierarchy(IEnumerable<Function> functionList, int parentId = 0)
    {
        var sortedFunctionList = new List<Function>();

        foreach (var function in functionList)
        {
            if (function.ParentId == parentId)
            {
                sortedFunctionList.Add(function);
                sortedFunctionList.AddRange(SortFunctionsInHierarchy(functionList, function.Id));
            }
        }

        return sortedFunctionList;
    }

    public CommandResult CopyPermission(CopyPermissionCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        var result= repository.Execute(criteria, logCriteria);

        DataCachingJobCommands.Instance.AddUserFunctionPolicyCommand(criteria.SiteId);

        return result;
    }
}
