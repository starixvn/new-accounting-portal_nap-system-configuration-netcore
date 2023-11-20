namespace NapAdmin.Controller;

using System;
using System.Collections.Generic;
using Fanex.NAP.Client.Attributes;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Business;
using Fanex.NAP.Client.Helpers.Web;
using Microsoft.AspNetCore.Mvc;
using NapAdminCore.Configuration;
using NapAdminCore.Criteria.MenuItem;
using NapAdminCore.Models;
using NapAdminCore.Service;
using NapAdminCore.Web;

[NapSuperAuth]
public class MenuController(IMenuItemService menuitemService,
        ISiteService siteService,
        IFunctionService functionService,
        IUserService userService,
        INapSession napSession,
        INapSettings napSettings)
    : BaseController(napSession, napSettings, userService)
{
    #region Manage Menu item

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Read)]
    public ActionResult Manage(int? siteId)
    {
        var criteria = new GetBySiteMenuItemCriteria
        {
            SiteId = siteId ?? SiteId,
            Environment = Environment
        };

        PrepareManageData(criteria);

        return View(criteria);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Read)]
    public ActionResult Manage(GetBySiteMenuItemCriteria criteria)
    {
        PrepareManageData(criteria);

        return View(criteria);
    }

    #endregion Manage Menu item

    #region Add New Menu

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
    public ActionResult AddNewMenuItem(int? siteId = null)
    {
        var criteria = new InsertMenuItemCriteria
        {
            Environment = Environment,
            Policy = (int)AdminConstant.Policy.Read,
            SiteId = siteId ?? SiteId
        };

        PrepareAddNewData(criteria);

        return View(criteria);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
    public ActionResult AddNewMenuItem(InsertMenuItemCriteria criteria)
    {
        PrepareAddNewData(criteria);

        if (ModelState.IsValid)
        {
            criteria.CreatedBy = NapSession.UserId;
            criteria.CreatedDate = DateTime.Now;
            criteria.ParentId = criteria.ParentId == 0 ? null : criteria.ParentId;
            criteria.ParentName = criteria.ParentId is 0 or null ? null : criteria.ParentName;

            var result = menuitemService.Insert(criteria);

            if (result is { IsSuccessful: true })
            {
                ViewBag.Success = true;
            }
        }

        return View(criteria);
    }

    #endregion Add New Menu

    #region Edit Menu Item

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
    public ActionResult EditMenuItem(int menuItemId)
    {
        var menuItem = menuitemService.GetById(menuItemId);

        if (menuItem == null)
        {
            return RedirectToAction("Manage");
        }

        var criteria = new UpdateMenuItemCriteria
        {
            MenuItemId = menuItem.Id,
            MenuItemName = menuItem.Name,
            Description = menuItem.Description,
            DisplayOrder = menuItem.DisplayOrder,
            FunctionId = menuItem.FunctionId,
            IsActive = menuItem.IsActive,
            ParentId = menuItem.ParentId,
            ParentName = menuItem.ParentId > 0 ? menuItem.ParentName : string.Empty,
            Policy = menuItem.Policy,
            SiteId = menuItem.SiteId,
            SiteName = menuItem.SiteName,
            Url = menuItem.Url,
            Environment = menuItem.Environment
        };

        ViewBag.Function = functionService.GetSelectListItemBySite(
            criteria.SiteId,
            criteria.Environment,
            isActive: true,
            selectedFunctionId: criteria.FunctionId);

        ViewBag.Parent = criteria.ParentId == null ?
            menuitemService.GetParentSelectListItemById(id: criteria.MenuItemId, env: criteria.Environment) :
            menuitemService.GetParentSelectListItemById(criteria.MenuItemId, (int)criteria.ParentId, criteria.Environment);

        return View(criteria);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
    public ActionResult EditMenuItem(UpdateMenuItemCriteria criteria)
    {
        ViewBag.Function = functionService.GetSelectListItemBySite(
            criteria.SiteId,
            criteria.Environment,
            isActive: true,
            selectedFunctionId: criteria.FunctionId);

        ViewBag.Parent = criteria.ParentId == null ?
            menuitemService.GetParentSelectListItemById(criteria.MenuItemId) :
            menuitemService.GetParentSelectListItemById(criteria.MenuItemId, (int)criteria.ParentId);

        if (ModelState.IsValid)
        {
            criteria.ModifierId = NapSession.UserId;
            criteria.ModifiedDate = DateTime.Now;
            criteria.ParentId = criteria.ParentId == 0 ? null : criteria.ParentId;
            criteria.ParentName = criteria.ParentId is 0 or null ? null
                : criteria.ParentName;

            var result = menuitemService.Update(criteria);

            if (result is { IsSuccessful: true })
            {
                ViewBag.Success = true;
            }
        }

        return View(criteria);
    }

    #endregion Edit Menu Item

    #region AjaxRequest

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
    public ActionResult ListMenuItemBySite(int siteId, int? menuItemId, string environment = EmptyString)
    {
        if (Request.IsAjaxRequest())
        {
            var menuItemList = new List<MenuItem>();

            var environmentValue = !string.IsNullOrEmpty(environment) ? environment : Environment;
            menuItemList.Add(new MenuItem { Id = 0, Name = ViewResources.None });

            var tempMenuItemList = menuItemId.HasValue
                ? menuitemService.GetParentById(menuItemId.Value, env: environmentValue).ToList()
                : menuitemService.GetBySite(siteId, environmentValue).ToList();
            menuItemList.AddRange(tempMenuItemList);

            return Json(menuItemList);
        }

        return Json(new List<MenuItem>());
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
    public ActionResult ListFunctionBySite(int siteId, string environment = EmptyString)
    {
        if (Request.IsAjaxRequest())
        {
            return Json(functionService.GetBySite(
                siteId,
                !string.IsNullOrEmpty(environment) ? environment : Environment,
                isActive: true));
        }

        return Json(null);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
    public ActionResult Activate(int id, bool isActive)
    {
        if (Request.IsAjaxRequest())
        {
            var criteria = new UpdateStatusMenuItemCriteria
            {
                IsActive = isActive,
                MenuItemId = id,
                ModifiedBy = NapSession.UserId,
                ModifiedDate = DateTime.Now
            };

            var result = menuitemService.UpdateStatus(criteria);

            if (result is { IsSuccessful: true })
            {
                return Json(AdminConstant.Ok);
            }
        }

        return new EmptyResult();
    }

    #endregion AjaxRequest

    private void PrepareManageData(GetBySiteMenuItemCriteria criteria)
    {
        if (NapSession.Level == (int)AdminConstant.Level.PortalAdmin)
        {
            ViewBag.Site = siteService.GetAllSelectListItem(criteria.Environment, true, criteria.SiteId);
            ViewFlags.HasEditRight = true;
        }
        else
        {
            ViewBag.Site =
                siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                    NapSession.UserId,
                    FunctionKeys.ManageMenu,
                    criteria.Environment,
                    selectedSiteId: criteria.SiteId);
            ViewFlags.HasEditRight =
                HasRight(FunctionKeys.ManageMenu, AdminConstant.Policy.Host)
                && HasRightOnSite(criteria.SiteId, FunctionKeys.ManageMenu, AdminConstant.Policy.Host);
        }

        ViewBag.MenuItemList = menuitemService.GetBySiteInHierarchy(criteria.SiteId, criteria.Environment);
    }

    private void PrepareAddNewData(InsertMenuItemCriteria criteria)
    {
        ViewBag.Parent = menuitemService.GetSelectListItemBySite(criteria.SiteId, Environment, isActive: true);
        ViewBag.Function = functionService.GetSelectListItemBySite(criteria.SiteId, Environment, isActive: true);

        ViewBag.Site = NapSession.Level == (int)AdminConstant.Level.PortalAdmin ?
            siteService.GetAllSelectListItem(criteria.Environment, true, criteria.SiteId) :
            siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                NapSession.UserId,
                FunctionKeys.ManageMenu,
                criteria.Environment,
                policy: AdminConstant.Policy.Host,
                selectedSiteId: criteria.SiteId);
    }
}
