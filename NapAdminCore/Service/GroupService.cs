
namespace NapAdminCore.Service;

using System.Collections.Generic;
using System.Linq;
using Criteria.Group;
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ValueObject;
using Criteria;
using NapApiCaching;
using Repository;

public interface IGroupService
{
    Group GetById(int id);

    Group GetByName(string name);

    IEnumerable<Group> SearchGroupExcludeFunction(string groupName, int siteId = 0, int functionId = 0);

    IList<SelectListItem> GetAllSelectListItem(bool? isActive = null, int selectedGroupId = 0);

    IEnumerable<GroupPolicy> GetByFunctionAndSite(int functionId, int siteId);

    IEnumerable<Group> GetAllWithNumOfUser(string name, string env = AdminConstant.Uat, bool? isActive = null);

    CommandResult AssignFunction(AssignFunctionGroupCommand criteria);

    CommandResult UpdateStatus(UpdateStatusGroupCommand criteria);

    CommandResult Insert(InsertGroupCommand criteria);

    CommandResult Update(UpdateGroupCommand criteria);
}

public class GroupService(UpdateRepository repository, IDynamicRepository dynamicRepository,
        LoggedUserContext loggedUserContext)
    : IGroupService
{
    public Group GetById(int id)
    {
        var criteria = new GetByIdGroupCriteria
        {
            GroupId = id
        };

        return dynamicRepository.Get<Group>(criteria);
    }

    public Group GetByName(string name)
    {
        var criteria = new GetByNameGroupCriteria
        {
            GroupName = name
        };

        return dynamicRepository.Get<Group>(criteria);
    }
    
    public IList<SelectListItem> GetAllSelectListItem(bool? isActive = null, int selectedGroupId = 0)
    {
        var criteria = new GetAllGroupCriteria
        {
            IsActive = isActive
        };


        var groupList = dynamicRepository.Fetch<Group>(criteria).OrderBy(g => g.Name);
        return groupList.Select(g => new SelectListItem
        {
            Text = g.Name,
            Value = g.Id.ToString(),
            Selected = g.Id == selectedGroupId
        }).ToList();
    }

    public IEnumerable<Group> GetAllWithNumOfUser(string name, string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetAllWithNumOfUserGroupCriteria
        {
            IsActive = isActive,
            GroupName = name,
            Environment = env
        };

        return dynamicRepository.Fetch<Group>(criteria).OrderBy(g => g.Name);
    }

    public CommandResult AssignFunction(AssignFunctionGroupCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        var result = repository.Execute(criteria, logCriteria);

        DataCachingJobCommands.Instance.AddUserFunctionPolicyCommand(criteria.SiteId);

        return result;
    }

    public CommandResult UpdateStatus(UpdateStatusGroupCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        var result = repository.Execute(criteria, logCriteria);
        DataCachingJobCommands.Instance.AddUserFunctionPolicyCommand(0);

        return result;
    }

    public CommandResult Insert(InsertGroupCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        var result = repository.Execute(criteria, logCriteria);
        DataCachingJobCommands.Instance.AddGroupCommand();

        return result;
    }

    public CommandResult Update(UpdateGroupCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        var result = repository.Execute(criteria, logCriteria);
        DataCachingJobCommands.Instance.AddGroupCommand();
        DataCachingJobCommands.Instance.AddUserFunctionPolicyCommand(0);
        return result;
    }

    public IEnumerable<GroupPolicy> GetByFunctionAndSite(int functionId, int siteId)
    {
        var criteria = new GetByFunctionAndSiteGroupCriteria
        {
            FunctionId = functionId,
            SiteId = siteId
        };

        var userLevelList = dynamicRepository.Fetch<GroupPolicy>(criteria);
        return userLevelList.OrderBy(g => g.Name).ToList();
    }

    public IEnumerable<Group> SearchGroupExcludeFunction(string groupName, int siteId = 0, int functionId = 0)
    {
        var criteria = new SearchGroupExcludeFunctionCriteria
        {
            GroupName = groupName,
            SiteId = siteId,
            FunctionId = functionId
        };

        return dynamicRepository.Fetch<Group>(criteria).OrderBy(g => g.Name);
    }
}
