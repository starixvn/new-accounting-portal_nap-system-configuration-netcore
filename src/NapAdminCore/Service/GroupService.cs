
namespace NapAdminCore.Service;

using System.Collections.Generic;
using System.Linq;
using Business;
using Command;
using Criteria.Group;
using Domain;
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.ValueObject;
using Repository;

public interface IGroupService : IBaseService
{
    IGroup GetByUser(int userId);

    IGroup GetById(int id);

    IGroup GetByName(string name);

    IEnumerable<IGroup> GetAll(bool? isActive = null);

    IEnumerable<IGroup> SearchGroupExcludeFunction(string groupName, int siteId = 0, int functionId = 0);

    IList<SelectListItem> GetAllSelectListItem(bool? isActive = null, int selectedGroupId = 0);

    IEnumerable<GroupPolicy> GetByFunctionAndSite(int functionId, int siteId);

    IEnumerable<IGroup> GetAllWithNumOfUser(string name, string env = Constant.Uat, bool? isActive = null);

    ICommandResult AssignFunction(AssignFunctionGroupCriteria criteria);

    ICommandResult UpdateStatus(UpdateStatusGroupCriteria criteria);

    ICommandResult Insert(InsertGroupCriteria criteria);

    ICommandResult Update(UpdateGroupCriteria criteria);
}

public class GroupService : BaseService, IGroupService
{
    private readonly IGroupRepository groupRepository;
    private readonly IDynamicRepository dynamicRepository;

    public GroupService(IGroupRepository groupRepository, IDynamicRepository dynamicRepository)
    {
        this.groupRepository = groupRepository;
        this.dynamicRepository = dynamicRepository;
    }

    public IGroup GetByUser(int userId)
    {
        var criteria = new GetByUserGroupCriteria
        {
            UserId = userId
        };

        return dynamicRepository.Get<IGroup>(criteria);
    }

    public IGroup GetById(int id)
    {
        var criteria = new GetByIdGroupCriteria
        {
            GroupId = id
        };

        return dynamicRepository.Get<IGroup>(criteria);
    }

    public IGroup GetByName(string name)
    {
        var criteria = new GetByNameGroupCriteria
        {
            GroupName = name
        };

        return dynamicRepository.Get<IGroup>(criteria);
    }

    public IEnumerable<IGroup> GetAll(bool? isActive = null)
    {
        var criteria = new GetAllGroupCriteria
        {
            IsActive = isActive
        };

        return dynamicRepository.Fetch<IGroup>(criteria);
    }

    public IList<SelectListItem> GetAllSelectListItem(bool? isActive = null, int selectedGroupId = 0)
    {
        var criteria = new GetAllGroupCriteria
        {
            IsActive = isActive
        };


        var groupList = dynamicRepository.Fetch<IGroup>(criteria).OrderBy(g => g.Name);
        return groupList.Select(g => new SelectListItem
        {
            Text = g.Name,
            Value = g.Id.ToString(),
            Selected = g.Id == selectedGroupId
        }).ToList();
    }

    public IEnumerable<IGroup> GetAllWithNumOfUser(string name, string env = Constant.Uat, bool? isActive = null)
    {
        var criteria = new GetAllWithNumOfUserGroupCriteria
        {
            IsActive = isActive,
            GroupName = name,
            Environment = env
        };

        return dynamicRepository.Fetch<IGroup>(criteria).OrderBy(g => g.Name);
    }

    public ICommandResult AssignFunction(AssignFunctionGroupCriteria criteria)
    {
        var logCriteria = GetLogExecSpCriteria();

        return groupRepository.Update(criteria, logCriteria);
    }

    public ICommandResult UpdateStatus(UpdateStatusGroupCriteria criteria)
    {
        var logCriteria = GetLogExecSpCriteria();

        return groupRepository.Update(criteria, logCriteria);
    }

    public ICommandResult Insert(InsertGroupCriteria criteria)
    {
        var logCriteria = GetLogExecSpCriteria();

        return groupRepository.Insert(criteria, logCriteria);
    }

    public ICommandResult Update(UpdateGroupCriteria criteria)
    {
        var logCriteria = GetLogExecSpCriteria();

        return groupRepository.Update(criteria, logCriteria);
    }

    public IEnumerable<GroupPolicy> GetByFunctionAndSite(int functionId, int siteId)
    {
        var criteria = new GetByFunctionAndSiteGroupCriteria
        {
            FunctionId = functionId,
            SiteId = siteId
        };

        return groupRepository.GetGroupPolicy(criteria).OrderBy(g => g.Group.Name);
    }

    public IEnumerable<IGroup> SearchGroupExcludeFunction(string groupName, int siteId = 0, int functionId = 0)
    {
        var criteria = new SearchGroupExcludeFunctionCriteria
        {
            GroupName = groupName,
            SiteId = siteId,
            FunctionId = functionId
        };

        return dynamicRepository.Fetch<IGroup>(criteria).OrderBy(g => g.Name);
    }
}
