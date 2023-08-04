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
using NapAdminCore.Domain;
using NapAdminCore.Models;
using NapAdminCore.Service;
using NapAdminCore.Web;

[NapSuperAuth]
public class MenuController : BaseController
{
    #region Properties

    private readonly IMenuItemService _menuItemService;
    private readonly ISiteService _siteService;
    private readonly IFunctionService _functionService;

    #endregion Properties

    #region Contructors
    

    public MenuController(
        IMenuItemService menuitemService,
        ISiteService siteService,
        IFunctionService functionService,
        IUserService userService,
        INapSession napSession,
    INapSettings napSettings) :
    base(napSession, napSettings, userService)
    {
        _menuItemService = menuitemService ;
        _siteService = siteService ;
        _functionService = functionService;
    }

    #endregion Contructors

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
            criteria.ParentName = criteria.ParentId == 0 || criteria.ParentId == null ? null : criteria.ParentName;

            var result = _menuItemService.Insert(criteria);

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
        var menuItem = _menuItemService.GetById(menuItemId);

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
            FunctionId = menuItem.Function.Id,
            IsActive = menuItem.IsActive,
            ParentId = menuItem.Parent?.Id ?? 0,
            ParentName = menuItem.Parent != null ? menuItem.Parent.Name : string.Empty,
            Policy = menuItem.Policy,
            SiteId = menuItem.Site.Id,
            SiteName = menuItem.Site.Name,
            Url = menuItem.Url,
            Environment = menuItem.Environment
        };

        ViewBag.Function = _functionService.GetSelectListItemBySite(
            criteria.SiteId,
            criteria.Environment,
            isActive: true,
            selectedFunctionId: criteria.FunctionId);

        ViewBag.Parent = criteria.ParentId == null ?
            _menuItemService.GetParentSelectListItemById(id: criteria.MenuItemId, env: criteria.Environment) :
            _menuItemService.GetParentSelectListItemById(criteria.MenuItemId, (int)criteria.ParentId, criteria.Environment);

        return View(criteria);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
    public ActionResult EditMenuItem(UpdateMenuItemCriteria criteria)
    {
        ViewBag.Function = _functionService.GetSelectListItemBySite(
            criteria.SiteId,
            criteria.Environment,
            isActive: true,
            selectedFunctionId: criteria.FunctionId);

        ViewBag.Parent = criteria.ParentId == null ?
            _menuItemService.GetParentSelectListItemById(criteria.MenuItemId) :
            _menuItemService.GetParentSelectListItemById(criteria.MenuItemId, (int)criteria.ParentId);

        if (ModelState.IsValid)
        {
            criteria.ModifierId = NapSession.UserId;
            criteria.ModifiedDate = DateTime.Now;
            criteria.ParentId = criteria.ParentId == 0 ? null : criteria.ParentId;
            criteria.ParentName = criteria.ParentId == 0 || criteria.ParentId == null ? null
                : criteria.ParentName;

            var result = _menuItemService.Update(criteria);

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
            var menuItemList = new List<IMenuItem>();

            var environmentValue = !string.IsNullOrEmpty(environment) ? environment : Environment;
            menuItemList.Add(new MenuItem { Id = 0, Name = ViewResources.None });

            var tempMenuItemList = menuItemId.HasValue
                ? _menuItemService.GetParentById(menuItemId.Value, env: environmentValue).ToList()
                : _menuItemService.GetBySite(siteId, environmentValue).ToList();
            menuItemList.AddRange(tempMenuItemList);

            return Json(menuItemList);
        }

        return Json(new List<IMenuItem>());
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageMenu, Policy = Constant.Policy.Host)]
    public ActionResult ListFunctionBySite(int siteId, string environment = EmptyString)
    {
        if (Request.IsAjaxRequest())
        {
            return Json(_functionService.GetBySite(
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

            var result = _menuItemService.UpdateStatus(criteria);

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
            ViewBag.Site = _siteService.GetAllSelectListItem(criteria.Environment, true, criteria.SiteId);
            ViewFlags.HasEditRight = true;
        }
        else
        {
            ViewBag.Site =
                _siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                    NapSession.UserId,
                    FunctionKeys.ManageMenu,
                    criteria.Environment,
                    selectedSiteId: criteria.SiteId);
            ViewFlags.HasEditRight =
                HasRight(FunctionKeys.ManageMenu, AdminConstant.Policy.Host)
                && HasRightOnSite(criteria.SiteId, FunctionKeys.ManageMenu, AdminConstant.Policy.Host);
        }

        ViewBag.MenuItemList = _menuItemService.GetBySiteInHierarchy(criteria.SiteId, criteria.Environment);
    }

    private void PrepareAddNewData(InsertMenuItemCriteria criteria)
    {
        ViewBag.Parent = _menuItemService.GetSelectListItemBySite(criteria.SiteId, Environment, isActive: true);
        ViewBag.Function = _functionService.GetSelectListItemBySite(criteria.SiteId, Environment, isActive: true);

        ViewBag.Site = NapSession.Level == (int)AdminConstant.Level.PortalAdmin ?
            _siteService.GetAllSelectListItem(criteria.Environment, true, criteria.SiteId) :
            _siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                NapSession.UserId,
                FunctionKeys.ManageMenu,
                criteria.Environment,
                policy: AdminConstant.Policy.Host,
                selectedSiteId: criteria.SiteId);
    }
}
