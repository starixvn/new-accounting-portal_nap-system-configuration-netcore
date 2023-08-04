
namespace NapAdminCore.Service;

using System.Collections.Generic;
using System.Linq;
using Command;
using Criteria.MenuItem;
using Domain;
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Repository;

public interface IMenuItemService 
{
    IMenuItem GetById(int id);

    IMenuItem GetByNameAndSite(string name, int siteId);

    IEnumerable<IMenuItem> GetAll(string env = AdminConstant.Uat, bool? isActive = null);

    IList<SelectListItem> GetAllSelectListItem(
        string env = AdminConstant.Uat,
        bool? isActive = null,
        int selectedMenuItemId = 0);

    IEnumerable<IMenuItem> GetBySite(int siteId, string env = AdminConstant.Uat, bool? isActive = null);

    IList<SelectListItem> GetSelectListItemBySite(
        int siteId,
        string env = AdminConstant.Uat,
        bool? isActive = null,
        int selectedMenuItemId = 0);

    IEnumerable<IMenuItem> GetBySiteInHierarchy(int siteId, string env = AdminConstant.Uat, bool? isActive = null);

    IEnumerable<IMenuItem> GetByUserAndSiteIncludeGroup(
        int userId,
        int siteId,
        string env = AdminConstant.Uat,
        bool? isActive = null);

    IEnumerable<IMenuItem> Search(
        string menuItemName,
        int siteId,
        string env = AdminConstant.Uat,
        bool? isActive = null);

    IEnumerable<IMenuItem> GetParentById(int id, string env = AdminConstant.Uat);

    IList<SelectListItem> GetParentSelectListItemById(int id, int selectedMenuItemId = 0, string env = AdminConstant.Uat);

    ICommandResult Insert(InsertMenuItemCriteria criteria);

    ICommandResult Update(UpdateMenuItemCriteria criteria);

    ICommandResult UpdateStatus(UpdateStatusMenuItemCriteria criteria);
}
public class MenuItemService : IMenuItemService
{
    private readonly IMenuItemRepository menuItemRepository;
    private readonly IDynamicRepository dynamicRepository;
    private readonly LoggedUserContext loggedUserContext;

    public MenuItemService(IMenuItemRepository menuItemRepository, IDynamicRepository dynamicRepository, LoggedUserContext loggedUserContext)
    {
        this.menuItemRepository = menuItemRepository;
        this.dynamicRepository = dynamicRepository;
        this.loggedUserContext = loggedUserContext;
    }

    public IMenuItem GetById(int id)
    {
        var criteria = new GetByIdMenuItemCriteria
        {
            MenuItemId = id
        };

        return dynamicRepository.Get<IMenuItem>(criteria);
    }

    public IMenuItem GetByNameAndSite(string name, int siteId)
    {
        var criteria = new GetByNameAndSiteMenuItemCriteria
        {
            MenuItemName = name,
            SiteId = siteId
        };

        return dynamicRepository.Get<IMenuItem>(criteria);
    }

    public IEnumerable<IMenuItem> GetAll(string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetAllMenuItemCriteria
        {
            Environment = env,
            IsActive = isActive
        };

        return dynamicRepository.Fetch<IMenuItem>(criteria);
    }

    public IList<SelectListItem> GetAllSelectListItem(
        string env = AdminConstant.Uat,
        bool? isActive = null,
        int selectedMenuItemId = 0)
    {
        var criteria = new GetAllMenuItemCriteria
        {
            Environment = env,
            IsActive = isActive
        };

        var menuItemList = dynamicRepository.Fetch<IMenuItem>(criteria);

        return menuItemList.Select(s => new SelectListItem
        {
            Text = s.Name,
            Value = s.Id.ToString(),
            Selected = s.Id == selectedMenuItemId
        }).ToList();
    }

    public IEnumerable<IMenuItem> GetBySite(int siteId, string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetBySiteMenuItemCriteria
        {
            SiteId = siteId,
            Environment = env,
            IsActive = isActive
        };

        return dynamicRepository.Fetch<IMenuItem>(criteria);
    }

    public IList<SelectListItem> GetSelectListItemBySite(
        int siteId,
        string env = AdminConstant.Uat,
        bool? isActive = null,
        int selectedMenuItemId = 0)
    {
        var criteria = new GetBySiteMenuItemCriteria
        {
            SiteId = siteId,
            Environment = env,
            IsActive = isActive
        };

        var menuItemList = dynamicRepository.Fetch<IMenuItem>(criteria);

        return menuItemList.Select(s => new SelectListItem
        {
            Text = s.Name,
            Value = s.Id.ToString(),
            Selected = s.Id == selectedMenuItemId
        }).ToList();
    }

    public IEnumerable<IMenuItem> GetBySiteInHierarchy(int siteId, string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetBySiteMenuItemCriteria
        {
            SiteId = siteId,
            Environment = env,
            IsActive = isActive
        };

        var menuItemList = dynamicRepository.Fetch<IMenuItem>(criteria).OrderBy(x => x.DisplayOrder);

        return SortMenuItemsInHierarchy(menuItemList);
    }

    public IEnumerable<IMenuItem> GetParentById(int id, string env = AdminConstant.Uat)
    {
        var criteria = new GetParentByIdMenuItemCriteria
        {
            MenuItemId = id,
            Environment = env
        };

        return dynamicRepository.Fetch<IMenuItem>(criteria);
    }

    public IList<SelectListItem> GetParentSelectListItemById(
        int id,
        int selectedMenuItemId = 0,
        string env = AdminConstant.Uat)
    {
        var criteria = new GetParentByIdMenuItemCriteria
        {
            MenuItemId = id,
            Environment = env
        };

        var menuItemList = dynamicRepository.Fetch<IMenuItem>(criteria);

        return menuItemList.Select(s => new SelectListItem
        {
            Text = s.Name,
            Value = s.Id.ToString(),
            Selected = s.Id == selectedMenuItemId
        }).ToList();
    }

    public IEnumerable<IMenuItem> GetByUserAndSiteIncludeGroup(
        int userId,
        int siteId,
        string env = AdminConstant.Uat,
        bool? isActive = null)
    {
        var criteria = new GetByUserAndSiteIncludeGroupMenuItemCriteria
        {
            UserId = userId,
            SiteId = siteId,
            Environment = env
        };

        return dynamicRepository.Fetch<IMenuItem>(criteria);
    }

    public IEnumerable<IMenuItem> Search(
        string menuItemName,
        int siteId,
        string env = AdminConstant.Uat,
        bool? isActive = null)
    {
        var criteria = new SearchAllMenuItemCriteria
        {
            MenuItemName = menuItemName,
            SiteId = siteId,
            Environment = env,
            IsActive = isActive
        };

        return dynamicRepository.Fetch<IMenuItem>(criteria);
    }

    public ICommandResult Insert(InsertMenuItemCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return menuItemRepository.Insert(criteria, logCriteria);
    }

    public ICommandResult Update(UpdateMenuItemCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return menuItemRepository.Update(criteria, logCriteria);
    }

    public ICommandResult UpdateStatus(UpdateStatusMenuItemCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return menuItemRepository.Update(criteria, logCriteria);
    }

    private IEnumerable<IMenuItem> SortMenuItemsInHierarchy(IEnumerable<IMenuItem> menuItemList, int parentId = 0)
    {
        var sortedMenuItemList = new List<IMenuItem>();

        foreach (var menuItem in menuItemList)
        {
            if (menuItem.Parent?.Id== parentId)
            {
                sortedMenuItemList.Add(menuItem);
                sortedMenuItemList.AddRange(SortMenuItemsInHierarchy(menuItemList, menuItem.Id));
            }
        }

        return sortedMenuItemList;
    }
}
