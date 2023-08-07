
namespace NapAdminCore.Service;

using System.Collections.Generic;
using System.Linq;
using Command;
using Criteria.MenuItem;
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Repository;

public interface IMenuItemService
{
    MenuItem GetById(int id);

    IEnumerable<MenuItem> GetBySite(int siteId, string env = AdminConstant.Uat, bool? isActive = null);

    IList<SelectListItem> GetSelectListItemBySite(
        int siteId,
        string env = AdminConstant.Uat,
        bool? isActive = null,
        int selectedMenuItemId = 0);

    IEnumerable<MenuItem> GetBySiteInHierarchy(int siteId, string env = AdminConstant.Uat, bool? isActive = null);

    IEnumerable<MenuItem> GetParentById(int id, string env = AdminConstant.Uat);

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

    public MenuItem GetById(int id)
    {
        var criteria = new GetByIdMenuItemCriteria
        {
            MenuItemId = id
        };

        return dynamicRepository.Get<MenuItem>(criteria);
    }

    public IEnumerable<MenuItem> GetBySite(int siteId, string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetBySiteMenuItemCriteria
        {
            SiteId = siteId,
            Environment = env,
            IsActive = isActive
        };

        return dynamicRepository.Fetch<MenuItem>(criteria);
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

        var menuItemList = dynamicRepository.Fetch<MenuItem>(criteria);

        return menuItemList.Select(s => new SelectListItem
        {
            Text = s.Name,
            Value = s.Id.ToString(),
            Selected = s.Id == selectedMenuItemId
        }).ToList();
    }

    public IEnumerable<MenuItem> GetBySiteInHierarchy(int siteId, string env = AdminConstant.Uat, bool? isActive = null)
    {
        var criteria = new GetBySiteMenuItemCriteria
        {
            SiteId = siteId,
            Environment = env,
            IsActive = isActive
        };

        var menuItemList = dynamicRepository.Fetch<MenuItem>(criteria).OrderBy(x => x.DisplayOrder);

        return SortMenuItemsInHierarchy(menuItemList);
    }

    public IEnumerable<MenuItem> GetParentById(int id, string env = AdminConstant.Uat)
    {
        var criteria = new GetParentByIdMenuItemCriteria
        {
            MenuItemId = id,
            Environment = env
        };

        return dynamicRepository.Fetch<MenuItem>(criteria);
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

        var menuItemList = dynamicRepository.Fetch<MenuItem>(criteria);

        return menuItemList.Select(s => new SelectListItem
        {
            Text = s.Name,
            Value = s.Id.ToString(),
            Selected = s.Id == selectedMenuItemId
        }).ToList();
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

    private IEnumerable<MenuItem> SortMenuItemsInHierarchy(IEnumerable<MenuItem> menuItemList, int parentId = 0)
    {
        var sortedMenuItemList = new List<MenuItem>();

        foreach (var menuItem in menuItemList)
        {
            if (menuItem.ParentId == parentId)
            {
                sortedMenuItemList.Add(menuItem);
                sortedMenuItemList.AddRange(SortMenuItemsInHierarchy(menuItemList, menuItem.Id));
            }
        }

        return sortedMenuItemList;
    }
}
