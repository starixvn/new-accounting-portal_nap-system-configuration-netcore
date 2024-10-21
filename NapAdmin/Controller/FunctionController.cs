namespace NapAdmin.Controller;

using System;
using System.Linq;
using Fanex.NAP.Client.Attributes;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Business;
using Fanex.NAP.Client.Helpers.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NapAdminCore.Configuration;
using NapAdminCore.Criteria.Function;
using NapAdminCore.Criteria.User;
using NapAdminCore.Helpers;
using NapAdminCore.Models;
using NapAdminCore.Models.ValueObject;
using NapAdminCore.Service;
using NapAdminCore.Web;

[NapSuperAuth]
public class FunctionController(IFunctionService functionService,
        ISiteService siteService,
        IGroupService groupService,
        INapSession napSession,
        INapSettings napSettings,
        IUserService userService)
    : BaseController(napSession, napSettings, userService)
{
    #region Manage

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Read)]
    public ActionResult Manage(int? siteId)
    {
        var criteria = new GetBySiteFunctionCriteria
        {
            SiteId = siteId ?? SiteId,
            Environment = Environment
        };

        PrepareManageData(criteria);

        return View(criteria);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Read)]
    public ActionResult Manage(GetBySiteFunctionCriteria criteria)
    {
        PrepareManageData(criteria);

        return View(criteria);
    }

    #endregion Manage

    #region Add new

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
    public ActionResult AddNew(int? siteId = null)
    {
        var criteria = new InsertFunctionCommand
        {
            SiteId = siteId ?? SiteId,
            Environment = Environment
        };

        GetSitesAndFunctionsForAddNew(criteria);

        return View(criteria);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
    public ActionResult AddNew(InsertFunctionCommand criteria)
    {
        GetSitesAndFunctionsForAddNew(criteria);

        if (ModelState.IsValid)
        {
            criteria.FunctionName = criteria.FunctionName.Trim();
            var function = functionService.GetByNameAndSite(criteria.FunctionName, criteria.SiteId);

            if (function != null)
            {
                ModelState.AddModelError("FunctionName", string.Format(ViewResources.DuplicateMsg, criteria.FunctionName));
                return View(criteria);
            }

            criteria.FunctionKey = ConvertToKey(criteria.FunctionName);
            function = functionService.GetByKeyAndSite(criteria.FunctionKey, criteria.SiteId);

            if (function != null)
            {
                ModelState.AddModelError("FunctionKey", string.Format(ViewResources.DuplicateMsg, criteria.FunctionKey));
                return View(criteria);
            }

            criteria.ParentId = criteria.ParentId == 0 ? null : criteria.ParentId;
            criteria.ParentName = criteria.ParentId is null or 0 ? null : criteria.ParentName;
            criteria.CreatedBy = NapSession.UserId;
            criteria.CreatedDate = DateTime.Now;

            var result = functionService.Insert(criteria);

            if (result is { IsSuccessful: true })
            {
                ViewBag.Success = true;
            }
        }

        return View(criteria);
    }

    #endregion Add new

    #region Duplicate Function

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
    public ActionResult Duplicate(int id, int siteId )
    {
        var function = functionService.GetById(id);

        if (function == null)
        {
            return NotFound();
        }

        ViewBag.OriginFunctionName = function.Name;

        var site = siteService.GetById(function.SiteId);

        var criteria = new DuplicateFunctionCommand
        {
            FunctionId = function.Id,
            FunctionName = function.Name + " Copy",
            Description = function.Description,
            SiteId = siteId,
            SiteName = site.Name,
            Readonly = function.Readonly,
            ParentId = function.ParentId,
            DisplayOrder = function.DisplayOrder,
            IsActive = function.IsActive,
            Environment = function.Environment,
            ReturnSiteId = siteId
        };

        GetSitesAndFunctionsForAddNew(criteria, getSiteList: false);

        return View(criteria);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
    public ActionResult Duplicate(DuplicateFunctionCommand criteria)
    {
        var function = functionService.GetById(criteria.FunctionId);

        if (function == null)
        {
            return NotFound();
        }

        ViewBag.OriginFunctionName = function.Name;
        criteria.SiteId = function.SiteId;
        GetSitesAndFunctionsForAddNew(criteria, getSiteList: false);

        if (ModelState.IsValid)
        {
            criteria.FunctionName = criteria.FunctionName.Trim();

            if (functionService.GetByNameAndSite(criteria.FunctionName, criteria.SiteId) != null)
            {
                ModelState.AddModelError("FunctionName", string.Format(ViewResources.DuplicateMsg, criteria.FunctionName));
                return View(criteria);
            }

            criteria.FunctionKey = ConvertToKey(criteria.FunctionName);

            if (functionService.GetByKeyAndSite(criteria.FunctionKey, criteria.SiteId) != null)
            {
                ModelState.AddModelError("FunctionKey", string.Format(ViewResources.DuplicateMsg, criteria.FunctionKey));
                return View(criteria);
            }

            criteria.DuplicateFunctionId = function.Id;
            criteria.ParentId = criteria.ParentId == 0 ? null : criteria.ParentId;
            criteria.ParentName = criteria.ParentId is null or 0 ? null : criteria.ParentName;
            criteria.CreatedBy = NapSession.UserId;
            criteria.CreatedDate = DateTime.Now;

            var result = functionService.Duplicate(criteria);

            if (result is { IsSuccessful: true })
            {
                ViewBag.Success = true;
            }
        }

        return View(criteria);
    }

    #endregion Duplicate Function

    #region Copy Permisison

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
    public ActionResult CopyPermission(int id, int? siteId = null)
    {
        var function = functionService.GetById(id);

        if (function == null)
        {
            return NotFound();
        }

        var criteria = new CopyPermissionCommand
        {
            ToFunctionId = function.Id,
            ToFunctionName = function.Name,
            SiteId = siteId ?? function.SiteId,
            Environment = function.Environment,
            ReturnSiteId = siteId ?? function.SiteId
        };

        ViewBag.FunctionList =
            functionService.GetSelectListItemBySite(criteria.SiteId, criteria.Environment, isActive: true);

        return View(criteria);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
    public ActionResult CopyPermission(CopyPermissionCommand criteria)
    {
        var function = functionService.GetById(criteria.ToFunctionId);

        if (function == null)
        {
            return NotFound();
        }

        ViewBag.FunctionList =
            functionService.GetSelectListItemBySite(criteria.SiteId, criteria.Environment, isActive: true);

        if (ModelState.IsValid)
        {
            criteria.ModifiedBy = NapSession.UserId;
            criteria.ModifiedDate = DateTime.Now;

            var result = functionService.CopyPermission(criteria);

            if (result is { IsSuccessful: true })
            {
                ViewBag.Success = true;
            }
        }

        return View(criteria);
    }

    #endregion Copy Permisison

    #region Update

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
    public ActionResult Edit(int id)
    {
        var function = functionService.GetById(id);

        if (ReferenceEquals(function, null))
        {
            return RedirectToAction("Manage");
        }

        var criteria = new UpdateFunctionCommand
        {
            FunctionId = function.Id,
            SiteId = function.SiteId,
            SiteName = function.SiteName,
            FunctionName = function.Name,
            FunctionKey = function.Key,
            Description = function.Description,
            ParentId = function.ParentId,
            ParentName = function.ParentName,
            Readonly = function.Readonly,
            DisplayOrder = function.DisplayOrder,
            IsActive = function.IsActive,
            Environment = function.Environment
        };

        var functionList = functionService.GetBySite(criteria.SiteId, criteria.Environment, true);

        ViewBag.ParentFunctionItemList = functionList
            .Where(f => f.Id != id && (f.ParentId == 0 || f.ParentId != id))
            .Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString(),
                Selected = function.ParentId > 0 && s.Id == function.ParentId
            }).ToList();

        return View(criteria);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
    public ActionResult Edit(UpdateFunctionCommand criteria)
    {
        var currentFunction = functionService.GetById(criteria.FunctionId);

        if (ReferenceEquals(currentFunction, null))
        {
            return RedirectToAction("Manage");
        }

        var functionList = functionService.GetBySite(criteria.SiteId, criteria.Environment, true);

        ViewBag.ParentFunctionItemList = functionList.Select(s => new SelectListItem
        {
            Text = s.Name,
            Value = s.Id.ToString(),
            Selected = criteria.ParentId != null && s.Id == criteria.ParentId
        }).ToList();

        if (ModelState.IsValid)
        {
            criteria.FunctionName = criteria.FunctionName.Trim();
            var function = functionService.GetByNameAndSite(criteria.FunctionName, criteria.SiteId);

            if (function != null && function.Id != criteria.FunctionId)
            {
                criteria.FunctionKey = currentFunction.Key;

                ModelState.AddModelError("FunctionName", string.Format(ViewResources.DuplicateMsg, criteria.FunctionName));

                return View(criteria);
            }

            if (criteria.FunctionKey != null)
            {
                function = functionService.GetByKeyAndSite(criteria.FunctionKey, criteria.SiteId);

                if (function != null && function.Id != criteria.FunctionId)
                {
                    ModelState.AddModelError("FunctionKey", string.Format(ViewResources.DuplicateMsg, criteria.FunctionKey));

                    return View(criteria);
                }
            }
            else
            {
                criteria.FunctionKey = ConvertToKey(criteria.FunctionName);
            }

            if (criteria.Crud < currentFunction.Crud &&
                NapSession.Level != (int)AdminConstant.Level.SiteAdmin &&
                NapSession.Level != (int)AdminConstant.Level.PortalAdmin)
            {
                var listUsers = UserService.GetByFunctionAndSite(currentFunction.Id, currentFunction.SiteId)
                    .Where(u => u.Policy == (int)AdminConstant.Policy.Host).ToList();

                var listGroups = groupService.GetByFunctionAndSite(currentFunction.Id, currentFunction.SiteId)
                    .Where(u => u.Policy == (int)AdminConstant.Policy.Host).ToList();

                if (listUsers.Count > 0 || listGroups.Count > 0)
                {
                    ModelState.AddModelError("Readonly", ViewResources.Privileged_User_Group_Message);

                    return View(criteria);
                }
            }

            criteria.ParentId = criteria.ParentId == 0 ? null : criteria.ParentId;
            criteria.ParentName = criteria.ParentId is null or 0 ? null : criteria.ParentName;
            criteria.ModifiedBy = NapSession.UserId;
            criteria.ModifiedDate = DateTime.Now;

            var result = functionService.Update(criteria);

            if (result is { IsSuccessful: true })
            {
                ViewBag.Success = true;
            }
        }

        return View(criteria);
    }

    #endregion Update

    #region Activation

    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
    public ActionResult Activate(int id, bool isActive)
    {
        if (Request.IsAjaxRequest())
        {
            var criteria = new UpdateStatusFunctionCommand
            {
                FunctionId = id,
                IsActive = isActive,
                ModifiedBy = NapSession.UserId,
                ModifiedDate = DateTime.Now
            };

            var result = functionService.UpdateStatus(criteria);

            if (result is { IsSuccessful: true })
            {
                return Json(AdminConstant.Ok);
            }
        }

        return new EmptyResult();
    }

    #endregion Activation

    #region Privileged Groups And Users

    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Read)]
    public ActionResult PrivilegedGroupsAndUsers(int id = 0)
    {
        var function = functionService.GetById(id);

        if (function == null)
        {
            return NotFound();
        }

        int siteId = function.SiteId;

        var users = UserService.GetByFunctionAndSite(id, siteId).ToList();
        var userSiteLevels = users.Select(u => u.UserId)
            .Distinct()
            .ToDictionary(u => u, u => UserService.GetUserSiteLevel(u, siteId));

        ViewBag.Function = function;
        ViewBag.Users = users;
        ViewBag.Groups = groupService.GetByFunctionAndSite(id, siteId).ToList();
        ViewBag.UserSiteLevels = userSiteLevels;

        ViewFlags.HasAssignFunctionToUserRight =
            HasRightOnSite(siteId, FunctionKeys.AssignFunctionToUser, AdminConstant.Policy.Host);
        ViewFlags.HasAssignFunctionToGroupRight =
            HasRightOnSite(siteId, FunctionKeys.AssignFunctionToGroup, AdminConstant.Policy.Host);

        ViewBag.GroupSelectList = groupService.GetAllSelectListItem();
        ViewBag.SiteId = siteId.ToString();

        return View();
    }

    [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToUser, Policy = Constant.Policy.Host)]
    public ActionResult PrivilegedUsers(
        int page = 1,
        int fid = 0,
        int gid = 0,
        string q = EmptyString)
    {
        const int size = 1000 * 1000;

        if (Request.IsAjaxRequest())
        {
            var function = functionService.GetById(fid);

            if (function == null)
            {
                return NotFound();
            }

            var criteria = new SearchUserExcludeFunctionCriteria
            {
                PageIndex = page,
                PageSize = size,
                Username = q.Trim(),
                GroupId = gid,
                SiteId = function.SiteId,
                FunctionId = fid,
                TotalRows = 0
            };

            var users = UserService.SearchUserExcludeFunction(criteria);

            var userList = new PagerList<UserLevel>(criteria.PageIndex, criteria.PageSize, criteria.TotalRows, users);

            var adminPolicy = UserService.GetUserFunctionPolicy(NapSession.UserId, fid);
            ViewBag.IsReadonlyAssign = adminPolicy < (int)AdminConstant.Policy.Host;

            return View(userList);
        }

        return new EmptyResult();
    }

    [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToUser, Policy = Constant.Policy.Host)]
    public string AddPrivilegedUsers(int id = 0, string readList = EmptyString, string hostList = EmptyString)
    {
        if (Request.IsAjaxRequest())
        {
            var function = functionService.GetById(id);

            if (function == null)
            {
                throw new ArgumentException($"Function (id={id}) not found");
            }

            var criteria = new AssignFunctionToUsersCommand
            {
                FunctionId = id,
                SiteId = function.SiteId,
                UserRead = readList.Split(',').ToList(),
                UserHost = hostList.Split(',').ToList(),
                ModifiedBy = NapSession.UserId,
                ModifiedDate = DateTime.Now
            };

            var result = functionService.AssignFunctionToUsers(criteria);

            if (result is { IsSuccessful: true })
            {
                return AdminConstant.Ok;
            }
        }

        return AdminConstant.Error;
    }

    [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToGroup, Policy = Constant.Policy.Host)]
    public ActionResult PrivilegedGroups(int fid = 0, string q = EmptyString)
    {
        if (Request.IsAjaxRequest())
        {
            var groupName = q.Trim();

            var function = functionService.GetById(fid);

            if (function == null)
            {
                return NotFound();
            }

            var group = groupService.SearchGroupExcludeFunction(groupName, function.SiteId, fid);

            var adminPolicy = UserService.GetUserFunctionPolicy(NapSession.UserId, fid);
            ViewBag.IsReadonlyAssign = adminPolicy < (int)AdminConstant.Policy.Host;

            return View(group);
        }

        return new EmptyResult();
    }

    [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToGroup, Policy = Constant.Policy.Host)]
    public string AddPrivilegedGroups(int id = 0, string readList = EmptyString, string hostList = EmptyString)
    {
        if (Request.IsAjaxRequest())
        {
            var function = functionService.GetById(id);

            if (function == null)
            {
                throw new ArgumentException($"Function (id={id}) not found");
            }

            var criteria = new AssignFunctionToGroupsCommand
            {
                FunctionId = id,
                SiteId = function.SiteId,
                GroupRead = readList.Split(',').ToList(),
                GroupHost = hostList.Split(',').ToList(),
                ModifiedBy = NapSession.UserId,
                ModifiedDate = DateTime.Now
            };

            var result = functionService.AssignFunctionToGroups(criteria);

            if (result is { IsSuccessful: true })
            {
                return AdminConstant.Ok;
            }
        }

        return AdminConstant.Error;
    }

    #endregion Privileged Groups And Users

    #region Ajax Request

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Read)]
    public JsonResult GetFunctionBySite(int siteId)
    {
        if (Request.IsAjaxRequest())
        {
            var functionList = functionService.GetBySite(siteId, Environment, true).ToList();

            functionList.Insert(0, new Function { Id = 0, Name = ViewResources.None });

            return Json(functionList);
        }

        return Json(null);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
    public JsonResult GetFunctionParentByEnvironment(int siteId = 0, int functionId = 0, string env = AdminConstant.Uat)
    {
        if (Request.IsAjaxRequest())
        {
            if (functionId != 0)
            {
                var function = functionService.GetById(functionId);
                var functionList = functionService.GetBySite(function.SiteId, env, true).ToList();

                functionList = functionList.Where(f => f.Id != functionId && (f.ParentId == 0
                                                                              || f.ParentId != functionId)).ToList();

                functionList.Insert(0, new Function { Id = 0, Name = ViewResources.None });

                return Json(functionList);
            }
            else
            {
                var functionList = functionService.GetBySite(siteId, env, true).ToList();

                functionList.Insert(0, new Function { Id = 0, Name = ViewResources.None });

                return Json(functionList);
            }
        }

        return Json(null);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
    public JsonResult GetFunctionKey(int functionId, string functionName)
    {
        var tempFunction = functionService.GetById(functionId);

        if (ReferenceEquals(tempFunction, null))
        {
            return Json(-1);
        }

        var function = functionService.GetByNameAndSite(functionName, tempFunction.SiteId);

        if (function != null && function.Id != tempFunction.Id)
        {
            return Json(0);
        }

        var functionKey = ConvertToKey(functionName);

        function = functionService.GetByKeyAndSite(functionKey, tempFunction.SiteId);

        if (function != null && function.Id != tempFunction.Id)
        {
            return Json(-2);
        }

        return Json(functionKey);
    }

    #endregion Ajax Request

    private void GetSitesAndFunctionsForAddNew(dynamic criteria, bool getSiteList = true)
    {
        if (getSiteList)
        {
            ViewBag.SiteItemList = UserIsPortalAdmin ?
                siteService.GetAllSelectListItem(criteria.Environment, isActive: true, selectedSiteId: criteria.SiteId) :
                siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                    NapSession.UserId,
                    FunctionKeys.ManageFunction,
                    criteria.Environment,
                    policy: AdminConstant.Policy.Host,
                    selectedSiteId: criteria.SiteId);
        }

        ViewBag.FunctionList = criteria.ParentId == null ?
            functionService.GetSelectListItemBySite(criteria.SiteId, criteria.Environment, isActive: true) :
            functionService.GetSelectListItemBySite(
                criteria.SiteId,
                criteria.Environment,
                isActive: true,
                selectedFunctionId: (int)criteria.ParentId);
    }

    private void PrepareManageData(GetBySiteFunctionCriteria criteria)
    {
        ViewBag.FunctionList = functionService.GetBySiteInHierarchy(criteria.SiteId, criteria.Environment);

        if (UserIsPortalAdmin)
        {
            ViewBag.SiteItemList = siteService.GetAllSelectListItem(criteria.Environment, true, criteria.SiteId);
            ViewBag.ViewFlags.HasEditRight = true;
        }
        else
        {
            ViewBag.SiteItemList =
                siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                    NapSession.UserId,
                    FunctionKeys.ManageFunction,
                    criteria.Environment,
                    selectedSiteId: criteria.SiteId);

            ViewFlags.HasEditRight =
                HasRight(FunctionKeys.ManageFunction, AdminConstant.Policy.Host)
                && HasRightOnSite(criteria.SiteId, FunctionKeys.ManageFunction, AdminConstant.Policy.Host);
        }
    }
}
