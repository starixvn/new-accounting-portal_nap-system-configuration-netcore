namespace NapAdmin.Controller;

using System;
using System.Collections.Generic;
using System.Linq;
using Fanex.Common.Extensions;
using Fanex.NAP.Client.Attributes;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Business;
using Fanex.NAP.Client.Helpers.Web;
using Microsoft.AspNetCore.Mvc;
using NapAdminCore.Configuration;
using NapAdminCore.Criteria.Group;
using NapAdminCore.Models;
using NapAdminCore.Models.ValueObject;
using NapAdminCore.Service;
using NapAdminCore.Web;

[NapSuperAuth]
public class GroupController(IGroupService groupService,
        ISiteService siteService,
        IFunctionService functionService,
        IUserService userService,
        INapSession napSession,
        INapSettings napSettings)
    : BaseController(napSession, napSettings, userService)
{
    #region Manage

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.ManageGroup, Policy = Constant.Policy.Read)]
    public ActionResult Manage()
    {
        ViewFlags.HasEditRight = HasRight(FunctionKeys.ManageGroup, AdminConstant.Policy.Host);
        ViewFlags.HasAssignFunctionRight = HasRight(FunctionKeys.AssignFunctionToGroup, AdminConstant.Policy.Read);
        ViewFlags.HasManageUserRight = HasRight(FunctionKeys.ManageUser, AdminConstant.Policy.Read);

        ViewBag.GroupList = groupService.GetAllWithNumOfUser(string.Empty, Environment);

        return View(new GetAllWithNumOfUserGroupCriteria());
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageGroup, Policy = Constant.Policy.Read)]
    public ActionResult Manage(GetAllWithNumOfUserGroupCriteria criteria)
    {
        criteria.Environment = Environment;

        ViewFlags.HasEditRight = HasRight(FunctionKeys.ManageGroup, AdminConstant.Policy.Host);
        ViewFlags.HasAssignFunctionRight = HasRight(FunctionKeys.AssignFunctionToGroup, AdminConstant.Policy.Read);
        ViewFlags.HasManageUserRight = HasRight(FunctionKeys.ManageUser, AdminConstant.Policy.Read);

        ViewBag.GroupList = groupService.GetAllWithNumOfUser(criteria.GroupName);

        return View(criteria);
    }

    #endregion Manage

    #region Add New Group

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.ManageGroup, Policy = Constant.Policy.Host)]
    public ActionResult AddNew()
    {
        return View(new InsertGroupCommand());
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageGroup, Policy = Constant.Policy.Host)]
    public ActionResult AddNew(InsertGroupCommand criteria)
    {
        if (ModelState.IsValid)
        {
            var group = groupService.GetByName(criteria.GroupName);

            if (!ReferenceEquals(group, null))
            {
                ModelState.AddModelError("SiteName", string.Format(ViewResources.DuplicateMsg, criteria.GroupName));
                return View(criteria);
            }

            criteria.CreatedBy = napSession.UserId;
            criteria.CreatedDate = DateTime.Now;

            var result = groupService.Insert(criteria);

            if (result is { IsSuccessful: true })
            {
                ViewBag.Success = true;
            }
        }

        return View(criteria);
    }

    #endregion Add New Group

    #region Edit Group

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.ManageGroup, Policy = Constant.Policy.Host)]
    public ActionResult Edit(int id)
    {
        var group = groupService.GetById(id);

        if (ReferenceEquals(group, null))
        {
            return RedirectToAction("Manage");
        }

        var criteria = new UpdateGroupCommand
        {
            GroupId = group.Id,
            Description = group.Description,
            GroupName = group.Name,
            IsActive = group.IsActive
        };

        return View(criteria);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageGroup, Policy = Constant.Policy.Host)]
    public ActionResult Edit(UpdateGroupCommand criteria)
    {
        if (ModelState.IsValid)
        {
            var group = groupService.GetByName(criteria.GroupName);

            if (!ReferenceEquals(group, null) && group.Id != criteria.GroupId)
            {
                ModelState.AddModelError("SiteName", string.Format(ViewResources.DuplicateMsg, criteria.GroupName));

                return View(criteria);
            }

            criteria.ModifiedBy = napSession.UserId;
            criteria.ModifiedDate = DateTime.Now;

            var result = groupService.Update(criteria);

            if (result is { IsSuccessful: true })
            {
                ViewBag.Success = true;
            }
        }

        return View(criteria);
    }

    #endregion Edit Group

    #region Activation

    [NapAuth(FunctionKey = FunctionKeys.ManageGroup, Policy = Constant.Policy.Host)]
    public ActionResult Activate(int id, bool isActive)
    {
        if (Request.IsAjaxRequest())
        {
            var criteria = new UpdateStatusGroupCommand
            {
                GroupId = id,
                IsActive = isActive,
                ModifiedBy = napSession.UserId,
                ModifiedDate = DateTime.Now
            };

            var result = groupService.UpdateStatus(criteria);

            if (result is { IsSuccessful: true })
            {
                return Json(AdminConstant.Ok);
            }
        }

        return new EmptyResult();
    }

    #endregion Activation

    #region Assign Permission

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToGroup, Policy = Constant.Policy.Read)]
    public ActionResult AssignPermission(int id, int siteId = 0)
    {
        var siteIdValue = siteId == 0 || SiteId != napSettings.SiteId ? SiteId : siteId;

        var group = groupService.GetById(id);
        var site = siteService.GetById(siteIdValue);

        if (group == null || site == null)
        {
            return RedirectToAction("Manage");
        }

        var criteria = new AssignFunctionGroupCommand
        {
            GroupId = group.Id,
            GroupName = group.Name,
            SiteId = site.Id
        };

        ViewFlags.HasAssignFunctionRight =
            HasRightOnSite(criteria.SiteId, FunctionKeys.AssignFunctionToGroup, AdminConstant.Policy.Host);

        ViewBag.SiteSelectList = napSession.Level == (int)AdminConstant.Level.PortalAdmin ?
            siteService.GetAllSelectListItem(Environment, true, criteria.SiteId) :
            siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(napSession.UserId,
                FunctionKeys.AssignFunctionToGroup,
                Environment,
                selectedSiteId: criteria.SiteId);

        ViewBag.FunctionList = functionService.GetBySiteInHierarchy(criteria.SiteId, Environment, true);
        ViewBag.GroupFuncPolicyList =
            functionService.GetByGroupAndSite(criteria.GroupId, criteria.SiteId, Environment);
        ViewBag.UserFuncPolicyList =
            functionService.GetByUserAndSiteIncludeGroup(napSession.UserId, criteria.SiteId, Environment);
        ViewBag.UserLevel = napSession.Level;

        return View(criteria);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToGroup, Policy = Constant.Policy.Host)]
    public ActionResult AssignPermission(AssignFunctionGroupCommand criteria)
    {
        var group = groupService.GetById(criteria.GroupId);
        var site = siteService.GetById(criteria.SiteId);

        if (group == null || site == null)
        {
            return RedirectToAction("Manage");
        }

        var policy = userService.GetUserSiteFunctionKeyPolicy(
            napSession.UserId,
            criteria.SiteId,
            FunctionKeys.AssignFunctionToGroup);

        if (policy < (int)AdminConstant.Policy.Host)
        {
            return RedirectToAction("Unauthorize", "Common");
        }

        ViewFlags.HasAssignFunctionRight = true;

        criteria.ModifiedBy = napSession.UserId;
        criteria.ModifiedDate = DateTime.Now;

        var listFunc =
            functionService.GetBySiteInHierarchy(criteria.SiteId, Environment, true).ToList();

        var listGroupFuncPolicy =
            functionService.GetByGroupAndSite(criteria.GroupId, criteria.SiteId, Environment).ToList();

        var listGroupFuncRemove = new List<FunctionPolicy>();
        listGroupFuncRemove.AddRange(listGroupFuncPolicy);

        var listCurrentUserFuncPolicy =
            functionService.GetByUserAndSiteIncludeGroup(napSession.UserId, criteria.SiteId, Environment).ToList();

        if (!criteria.FuncHost.IsNullOrDBNull())
        {
            foreach (var itemFunc in criteria.FuncHost)
            {
                if (listFunc.All(f => f.Id != itemFunc))
                {
                    return RedirectToAction("Error", "Common");
                }

                listGroupFuncRemove.Remove(f => f.Id == itemFunc);
            }
        }

        if (!criteria.FuncRead.IsNullOrDBNull())
        {
            foreach (var itemFunc in criteria.FuncRead)
            {
                listGroupFuncRemove.Remove(f => f.Id == itemFunc);
            }
        }

        listGroupFuncRemove.Remove(f => f.Policy > 0);
        if (listGroupFuncRemove.Any(itemFunc => !listCurrentUserFuncPolicy.Any(f => f.Id == itemFunc.Id && f.Policy >= itemFunc.Policy)))
        {
            return RedirectToAction("Error", "Common");
        }

        var result = groupService.AssignFunction(criteria);

        if (result is { IsSuccessful: true })
        {
            ViewBag.Success = true;

            listGroupFuncPolicy =
                functionService.GetByGroupAndSite(criteria.GroupId, criteria.SiteId, Environment).ToList();
        }

        ViewBag.SiteSelectList = UserIsPortalAdmin ?
            siteService.GetAllSelectListItem(Environment, true, criteria.SiteId) :
            siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                napSession.UserId,
                FunctionKeys.AssignFunctionToGroup,
                Environment,
                selectedSiteId: criteria.SiteId);

        ViewBag.FunctionList = listFunc;
        ViewBag.GroupFuncPolicyList = listGroupFuncPolicy;
        ViewBag.UserFuncPolicyList = listCurrentUserFuncPolicy;
        ViewBag.UserLevel = napSession.Level;

        return View(criteria);
    }

    #endregion Assign Permission

    #region UnassignPermission
    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToGroup, Policy = Constant.Policy.Host)]
    public ActionResult UnassignPermission(int groupId, int functionId)
    {
        var function = functionService.GetById(functionId);
        var group = groupService.GetById(groupId);
        
        var listGroupFuncPolicy = functionService.GetByGroupAndSite(groupId, function.SiteId, Environment).ToList();

        var criteria = new AssignFunctionGroupCommand
        {
            FuncHost = listGroupFuncPolicy.Where(p => p.Policy == 2 && p.Id != functionId).Select(p => (int)p.Id).ToArray(),
            FuncRead = listGroupFuncPolicy.Where(p => p.Policy == 1 && p.Id != functionId).Select(p => (int)p.Id).ToArray(),
            ModifiedBy = napSession.UserId,
            ModifiedDate = DateTime.Now,
            GroupId = groupId,
            GroupName = group.Name,
            SiteId = function.SiteId
        };


        var result = groupService.AssignFunction(criteria);

        return Json(result);
    }

    #endregion
    #region Group Info

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageGroup, Policy = Constant.Policy.Read)]
    public ActionResult GroupInfo(int id)
    {
        var group = groupService.GetById(id);
        return View(group);
    }

    #endregion Group Info
}
