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
public class FunctionController : BaseController
{
    #region Members

    private readonly IFunctionService _functionService;
    private readonly ISiteService _siteService;
    private readonly IUserService _userService;
    private readonly IGroupService _groupService;

    #endregion Members

    #region Constructors
    
    public FunctionController(
        IFunctionService functionService,
        ISiteService siteService,
        IUserService userService,
        IGroupService groupService,
        INapSession napSession,
        INapSettings napSettings) :
        base(napSession, napSettings, userService)
    {
        _functionService = functionService;
        _siteService = siteService;
        _userService = userService;
        _groupService = groupService;
    }

    #endregion Constructors

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
        var criteria = new InsertFunctionCriteria
        {
            SiteId = siteId ?? SiteId,
            Environment = Environment
        };

        GetSitesAndFunctionsForAddNew(criteria);

        return View(criteria);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
    public ActionResult AddNew(InsertFunctionCriteria criteria)
    {
        GetSitesAndFunctionsForAddNew(criteria);

        if (ModelState.IsValid)
        {
            criteria.FunctionName = criteria.FunctionName.Trim();
            var function = _functionService.GetByNameAndSite(criteria.FunctionName, criteria.SiteId);

            if (function != null)
            {
                ModelState.AddModelError("FunctionName", string.Format(ViewResources.DuplicateMsg, criteria.FunctionName));
                return View(criteria);
            }

            criteria.FunctionKey = ConvertToKey(criteria.FunctionName);
            function = _functionService.GetByKeyAndSite(criteria.FunctionKey, criteria.SiteId);

            if (function != null)
            {
                ModelState.AddModelError("FunctionKey", string.Format(ViewResources.DuplicateMsg, criteria.FunctionKey));
                return View(criteria);
            }

            criteria.ParentId = criteria.ParentId == 0 ? null : criteria.ParentId;
            criteria.ParentName = (criteria.ParentId == null || criteria.ParentId == 0) ? null : criteria.ParentName;
            criteria.CreatedBy = NapSession.UserId;
            criteria.CreatedDate = DateTime.Now;

            var result = _functionService.Insert(criteria);

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
    public ActionResult Duplicate(int id, int? siteId = null)
    {
        var function = _functionService.GetById(id);

        if (function == null)
        {
            return NotFound();
        }

        ViewBag.OriginFunctionName = function.Name;

        var site = _siteService.GetById(function.Site.Id);

        var criteria = new DuplicateFunctionCriteria
        {
            FunctionId = function.Id,
            FunctionName = function.Name + " Copy",
            Description = function.Description,
            SiteId = SiteId,
            SiteName = site.Name,
            Readonly = function.Readonly,
            ParentId = function.Parent?.Id,
            DisplayOrder = function.DisplayOrder,
            IsActive = function.IsActive,
            Environment = function.Environment,
            ReturnSiteId = siteId ?? function.Site.Id
        };

        GetSitesAndFunctionsForAddNew(criteria, getSiteList: false);

        return View(criteria);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
    public ActionResult Duplicate(DuplicateFunctionCriteria criteria)
    {
        var function = _functionService.GetById(criteria.FunctionId);

        if (function == null)
        {
            return NotFound();
        }

        ViewBag.OriginFunctionName = function.Name;
        criteria.SiteId = function.Site.Id;
        GetSitesAndFunctionsForAddNew(criteria, getSiteList: false);

        if (ModelState.IsValid)
        {
            criteria.FunctionName = criteria.FunctionName.Trim();

            if (_functionService.GetByNameAndSite(criteria.FunctionName, criteria.SiteId) != null)
            {
                ModelState.AddModelError("FunctionName", string.Format(ViewResources.DuplicateMsg, criteria.FunctionName));
                return View(criteria);
            }

            criteria.FunctionKey = ConvertToKey(criteria.FunctionName);

            if (_functionService.GetByKeyAndSite(criteria.FunctionKey, criteria.SiteId) != null)
            {
                ModelState.AddModelError("FunctionKey", string.Format(ViewResources.DuplicateMsg, criteria.FunctionKey));
                return View(criteria);
            }

            criteria.DuplicateFunctionId = function.Id;
            criteria.ParentId = criteria.ParentId == 0 ? null : criteria.ParentId;
            criteria.ParentName = (criteria.ParentId == null || criteria.ParentId == 0) ? null : criteria.ParentName;
            criteria.CreatedBy = NapSession.UserId;
            criteria.CreatedDate = DateTime.Now;

            var result = _functionService.Duplicate(criteria);

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
        var function = _functionService.GetById(id);

        if (function == null)
        {
            return NotFound();
        }

        var criteria = new CopyPermissionCriteria
        {
            ToFunctionId = function.Id,
            ToFunctionName = function.Name,
            SiteId = siteId ?? function.Site.Id,
            Environment = function.Environment,
            ReturnSiteId = siteId ?? function.Site.Id
        };

        ViewBag.FunctionList =
            _functionService.GetSelectListItemBySite(criteria.SiteId, criteria.Environment, isActive: true);

        return View(criteria);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
    public ActionResult CopyPermission(CopyPermissionCriteria criteria)
    {
        var function = _functionService.GetById(criteria.ToFunctionId);

        if (function == null)
        {
            return NotFound();
        }

        ViewBag.FunctionList =
            _functionService.GetSelectListItemBySite(criteria.SiteId, criteria.Environment, isActive: true);

        if (ModelState.IsValid)
        {
            criteria.ModifiedBy = NapSession.UserId;
            criteria.ModifiedDate = DateTime.Now;

            var result = _functionService.CopyPermission(criteria);

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
        var function = _functionService.GetById(id);

        if (ReferenceEquals(function, null))
        {
            return RedirectToAction("Manage");
        }

        var criteria = new UpdateFunctionCriteria
        {
            FunctionId = function.Id,
            SiteId = function.Site.Id,
            SiteName = function.Site.Name,
            FunctionName = function.Name,
            FunctionKey = function.Key,
            Description = function.Description,
            ParentId = function.Parent?.Id,
            ParentName = function.Parent?.Name,
            Readonly = function.Readonly,
            DisplayOrder = function.DisplayOrder,
            IsActive = function.IsActive,
            Environment = function.Environment
        };

        var functionList = _functionService.GetBySite(criteria.SiteId, criteria.Environment, true);

        ViewBag.ParentFunctionItemList = functionList
            .Where(f => f.Id != id && (f.Parent == null || f.Parent.Id != id))
            .Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString(),
                Selected = function.Parent != null && s.Id == function.Parent.Id
            }).ToList();

        return View(criteria);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageFunction, Policy = Constant.Policy.Host)]
    public ActionResult Edit(UpdateFunctionCriteria criteria)
    {
        var currentFunction = _functionService.GetById(criteria.FunctionId);

        if (ReferenceEquals(currentFunction, null))
        {
            return RedirectToAction("Manage");
        }

        var functionList = _functionService.GetBySite(criteria.SiteId, criteria.Environment, true);

        ViewBag.ParentFunctionItemList = functionList.Select(s => new SelectListItem
        {
            Text = s.Name,
            Value = s.Id.ToString(),
            Selected = criteria.ParentId != null && s.Id == criteria.ParentId
        }).ToList();

        if (ModelState.IsValid)
        {
            criteria.FunctionName = criteria.FunctionName.Trim();
            var function = _functionService.GetByNameAndSite(criteria.FunctionName, criteria.SiteId);

            if (function != null && function.Id != criteria.FunctionId)
            {
                criteria.FunctionKey = currentFunction.Key;

                ModelState.AddModelError("FunctionName", string.Format(ViewResources.DuplicateMsg, criteria.FunctionName));

                return View(criteria);
            }

            if (criteria.FunctionKey != null)
            {
                function = _functionService.GetByKeyAndSite(criteria.FunctionKey, criteria.SiteId);

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
                var listUsers = _userService.GetByFunctionAndSite(currentFunction.Id, currentFunction.Site.Id)
                    .Where(u => u.Policy == (int)AdminConstant.Policy.Host).ToList();

                var listGroups = _groupService.GetByFunctionAndSite(currentFunction.Id, currentFunction.Site.Id)
                    .Where(u => u.Policy == (int)AdminConstant.Policy.Host).ToList();

                if (listUsers.Count > 0 || listGroups.Count > 0)
                {
                    ModelState.AddModelError("Readonly", ViewResources.Privileged_User_Group_Message);

                    return View(criteria);
                }
            }

            criteria.ParentId = criteria.ParentId == 0 ? null : criteria.ParentId;
            criteria.ParentName = criteria.ParentId == null || criteria.ParentId == 0 ? null : criteria.ParentName;
            criteria.ModifiedBy = NapSession.UserId;
            criteria.ModifiedDate = DateTime.Now;

            var result = _functionService.Update(criteria);

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
            var criteria = new UpdateStatusFunctionCriteria
            {
                FunctionId = id,
                IsActive = isActive,
                ModifiedBy = NapSession.UserId,
                ModifiedDate = DateTime.Now
            };

            var result = _functionService.UpdateStatus(criteria);

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
        var function = _functionService.GetById(id);

        if (function == null)
        {
            return NotFound();
        }

        int siteId = function.Site.Id;

        var users = _userService.GetByFunctionAndSite(id, siteId).ToList();
        var userSiteLevels = users.Select(u => u.User.Id)
            .Distinct()
            .ToDictionary(u => u, u => _userService.GetUserSiteLevel(u, siteId));

        ViewBag.Function = function;
        ViewBag.Users = users;
        ViewBag.Groups = _groupService.GetByFunctionAndSite(id, siteId).ToList();
        ViewBag.UserSiteLevels = userSiteLevels;

        ViewFlags.HasAssignFunctionToUserRight =
            HasRightOnSite(siteId, FunctionKeys.AssignFunctionToUser, AdminConstant.Policy.Host);
        ViewFlags.HasAssignFunctionToGroupRight =
            HasRightOnSite(siteId, FunctionKeys.AssignFunctionToGroup, AdminConstant.Policy.Host);

        ViewBag.GroupSelectList = _groupService.GetAllSelectListItem();
        ViewBag.SiteId = siteId.ToString();

        return View();
    }

    [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToUser, Policy = Constant.Policy.Host)]
    public ActionResult PrivilegedUsers(
        int id = 1,
        int fid = 0,
        int gid = 0,
        string q = EmptyString)
    {
        const int size = 1000 * 1000;

        if (Request.IsAjaxRequest())
        {
            var function = _functionService.GetById(fid);

            if (function == null)
            {
                return NotFound();
            }

            var criteria = new SearchUserExcludeFunctionCriteria
            {
                PageIndex = id,
                PageSize = size,
                Username = q.Trim(),
                GroupId = gid,
                SiteId = function.Site.Id,
                FunctionId = fid,
                TotalRows = 0
            };

            var users = _userService.SearchUserExcludeFunction(criteria);

            var userList = new PagerList<UserLevel>(criteria.PageIndex, criteria.PageSize, criteria.TotalRows, users);

            var adminPolicy = _userService.GetUserFunctionPolicy(NapSession.UserId, fid);
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
            var functionId = id;
            var function = _functionService.GetById(functionId);

            if (function == null)
            {
                throw new ArgumentException($"Function (id={id}) not found");
            }

            var criteria = new AssignFunctionToUsersCriteria
            {
                FunctionId = id,
                SiteId = function.Site.Id,
                UserRead = readList.Split(',').ToList(),
                UserHost = hostList.Split(',').ToList(),
                ModifiedBy = NapSession.UserId,
                ModifiedDate = DateTime.Now
            };

            var result = _functionService.AssignFunctionToUsers(criteria);

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
            var functionId = fid;
            var groupName = q.Trim();

            var function = _functionService.GetById(functionId);

            if (function == null)
            {
                return NotFound();
            }

            var group = _groupService.SearchGroupExcludeFunction(groupName, function.Site.Id, fid);

            var adminPolicy = _userService.GetUserFunctionPolicy(NapSession.UserId, fid);
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
            var functionId = id;
            var function = _functionService.GetById(functionId);

            if (function == null)
            {
                throw new ArgumentException($"Function (id={id}) not found");
            }

            var criteria = new AssignFunctionToGroupsCriteria
            {
                FunctionId = id,
                SiteId = function.Site.Id,
                GroupRead = readList.Split(',').ToList(),
                GroupHost = hostList.Split(',').ToList(),
                ModifiedBy = NapSession.UserId,
                ModifiedDate = DateTime.Now
            };

            var result = _functionService.AssignFunctionToGroups(criteria);

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
            var functionList = _functionService.GetBySite(siteId, Environment, true).ToList();

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
                var function = _functionService.GetById(functionId);
                var functionList = _functionService.GetBySite(function.Site.Id, env, true).ToList();

                functionList = functionList.Where(f => f.Id != functionId && (f.Parent == null
                                                                              || f.Parent.Id != functionId)).ToList();

                functionList.Insert(0, new Function { Id = 0, Name = ViewResources.None });

                return Json(functionList);
            }
            else
            {
                var functionList = _functionService.GetBySite(siteId, env, true).ToList();

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
        var tempFunction = _functionService.GetById(functionId);

        if (ReferenceEquals(tempFunction, null))
        {
            return Json(-1);
        }

        var function = _functionService.GetByNameAndSite(functionName, tempFunction.Site.Id);

        if (function != null && function.Id != tempFunction.Id)
        {
            return Json(0);
        }

        var functionKey = ConvertToKey(functionName);

        function = _functionService.GetByKeyAndSite(functionKey, tempFunction.Site.Id);

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
                _siteService.GetAllSelectListItem(criteria.Environment, isActive: true, selectedSiteId: criteria.SiteId) :
                _siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                    NapSession.UserId,
                    FunctionKeys.ManageFunction,
                    criteria.Environment,
                    policy: AdminConstant.Policy.Host,
                    selectedSiteId: criteria.SiteId);
        }

        ViewBag.FunctionList = criteria.ParentId == null ?
            _functionService.GetSelectListItemBySite(criteria.SiteId, criteria.Environment, isActive: true) :
            _functionService.GetSelectListItemBySite(
                criteria.SiteId,
                criteria.Environment,
                isActive: true,
                selectedFunctionId: (int)criteria.ParentId);
    }

    private void PrepareManageData(GetBySiteFunctionCriteria criteria)
    {
        ViewBag.FunctionList = _functionService.GetBySiteInHierarchy(criteria.SiteId, criteria.Environment);

        if (UserIsPortalAdmin)
        {
            ViewBag.SiteItemList = _siteService.GetAllSelectListItem(criteria.Environment, true, criteria.SiteId);
            ViewBag.ViewFlags.HasEditRight = true;
        }
        else
        {
            ViewBag.SiteItemList =
                _siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
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
