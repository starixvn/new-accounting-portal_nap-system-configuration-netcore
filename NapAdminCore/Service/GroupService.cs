
namespace NapAdminCore.Service;

using System.Collections.Generic;
using System.Linq;
using Command;
using Criteria.Group;
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ValueObject;
using Repository;

public interface IGroupService
{
    Group GetById(int id);

    Group GetByName(string name);

    IEnumerable<Group> SearchGroupExcludeFunction(string groupName, int siteId = 0, int functionId = 0);

    IList<SelectListItem> GetAllSelectListItem(bool? isActive = null, int selectedGroupId = 0);

    IEnumerable<GroupPolicy> GetByFunctionAndSite(int functionId, int siteId);

    IEnumerable<Group> GetAllWithNumOfUser(string name, string env = AdminConstant.Uat, bool? isActive = null);

    ICommandResult AssignFunction(AssignFunctionGroupCriteria criteria);

    ICommandResult UpdateStatus(UpdateStatusGroupCriteria criteria);

    ICommandResult Insert(InsertGroupCriteria criteria);

    ICommandResult Update(UpdateGroupCriteria criteria);
}

public class GroupService(IGroupRepository groupRepository, IDynamicRepository dynamicRepository,
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

    public ICommandResult AssignFunction(AssignFunctionGroupCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return groupRepository.Execute(criteria, logCriteria);
    }

    public ICommandResult UpdateStatus(UpdateStatusGroupCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return groupRepository.Execute(criteria, logCriteria);
    }

    public ICommandResult Insert(InsertGroupCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return groupRepository.Execute(criteria, logCriteria);
    }

    public ICommandResult Update(UpdateGroupCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return groupRepository.Execute(criteria, logCriteria);
    }

    public IEnumerable<GroupPolicy> GetByFunctionAndSite(int functionId, int siteId)
    {
        var criteria = new GetByFunctionAndSiteGroupCriteria
        {
            FunctionId = functionId,
            SiteId = siteId
        };

        return groupRepository.GetGroupPolicy(criteria).OrderBy(g => g.Name);
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
