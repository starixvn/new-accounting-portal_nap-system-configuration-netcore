namespace NapAdmin.Controller;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fanex.Common.Extensions;
using Fanex.Data.Repository;
using Fanex.NAP.Client.Attributes;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Business;
using Fanex.NAP.Client.Helpers.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NapAdminCore.Configuration;
using NapAdminCore.Criteria.User;
using NapAdminCore.DbParams;
using NapAdminCore.Helpers;
using NapAdminCore.Models;
using NapAdminCore.Models.ValueObject;
using NapAdminCore.Service;
using NapAdminCore.Web;
using Newtonsoft.Json;
using ONEbook.OTP.Services;
using ViewModels.User;

[NapSuperAuth]
public class UserController(IUserService userService,
        IGroupService groupService,
        ISiteService siteService,
        INapSession napSession,
        INapSettings napSettings,
        IFunctionService functionService,
        LoggedUserContext loggedUserContext)
    : BaseController(napSession, napSettings, userService)
{
    #region Manage

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Read)]
    public ActionResult Manage(
        int page = 1,
        int size = 20,
        string userName = EmptyString,
        int groupId = 0,
        int siteId = -1,
        int statusId = 1)
    {
        var username = ViewBag.Username = userName.Trim();
        siteId = siteId <= 0 && siteId != AdminConstant.SearchAllSite ? SiteId : siteId;
        
        ViewBag.IsPortalAdmin = userService.GetUserSiteLevel(NapSession.UserId, NapAdminId)
                                == (int)AdminConstant.Level.PortalAdmin;
        
        var listSite = GetSiteListForManageUser(siteId);
        var listSiteId = listSite.Select(i=>int.Parse(i.Value)).ToList();

        ViewBag.SiteSelectList = listSite; 
        ViewBag.GroupSelectList = groupService.GetAllSelectListItem(selectedGroupId: groupId);
        ViewBag.UserId = NapSession.UserId;
        ViewBag.ReturnGroupId = groupId;
        ViewBag.SiteId = siteId;

        ViewBag.HashLevel = GetHashLevel(listSiteId);
        
        var hashManageUserPolicy =
            userService.GetUserListSiteFunctionKeyPolicy(NapSession.UserId, FunctionKeys.ManageUser, listSite);

        var hashAssignFunctionPolicy = userService.GetUserListSiteFunctionKeyPolicy(
            NapSession.UserId,
            FunctionKeys.AssignFunctionToUser,
            listSite);

        var hashAssignSite = userService.GetUserListSiteFunctionKeyPolicy(
            NapSession.UserId,
            FunctionKeys.AssignSiteToUser,
            listSite);

        ViewBag.HashManageUser = hashManageUserPolicy;
        ViewBag.HashAssignFunction = hashAssignFunctionPolicy;
        ViewBag.HashAssignSite = hashAssignSite;

        ViewBag.ViewFlagsCurrentSite = new ViewFlags
        {
            HasManageUserRight = HasRight(FunctionKeys.ManageUser, AdminConstant.Policy.Host),
            HasAssignFunctionRight = HasRight(FunctionKeys.AssignFunctionToUser, AdminConstant.Policy.Read),
            HasAssignSiteRight = HasRight(FunctionKeys.AssignSiteToUser, AdminConstant.Policy.Read)
        };


        if (siteId == AdminConstant.SearchAllSite)
        {
            var searchAllSiteCriteria = new GetAllByUsernameGroupSiteUserCriteria
            {
                PageIndex = page,
                PageSize = size,
                Username = username,
                GroupId = groupId,
                ListSite = listSiteId,
                Status = statusId,
                TotalRows = 0,
                Environment = Environment
            };

            var users = userService.GetAllByUsernameGroupSite(searchAllSiteCriteria);

            return View(new PagerList<UserLevel>(page, size, searchAllSiteCriteria.TotalRows, users));
        }
        else
        {
            var criteria = new SearchUserCriteria
            {
                PageIndex = page,
                PageSize = size,
                Username = username,
                GroupId = groupId,
                SiteId = siteId,
                Status = statusId,
                TotalRows = 0,
                Environment = Environment
            };

            var users = userService.SearchUser(criteria);
            return View(new PagerList<UserLevel>(page, size, criteria.TotalRows, users));
        }
    }

    private Hashtable GetHashLevel(List<int> listSiteId)
    {
        var hashLevel = new Hashtable();
        foreach (var item in listSiteId)
        {
            var level = userService.GetUserSiteLevel(NapSession.UserId, item);
            hashLevel.Add(item, level);
        }

        return hashLevel;
    }

    private List<SelectListItem> GetSiteListForManageUser(int siteIdValue)
    {
        var listSite = UserIsPortalAdmin
            ? siteService.GetAllSelectListItem(Environment, true, siteIdValue).ToList()
            : siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                NapSession.UserId,
                FunctionKeys.ManageUser,
                Environment,
                selectedSiteId: siteIdValue).ToList();

        var siteSelectList = new List<SelectListItem>
        {
            new() { Text = ViewResources.All_Site, Value = AdminConstant.SearchAllSite.ToString() }
        };
        siteSelectList.AddRange(listSite);

        return siteSelectList;
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Read)]
    public ActionResult Manage(
        int size = 20,
        string userName = EmptyString,
        int groupId = 0,
        int siteId = -1,
        int statusId = 1)
    {
        return RedirectToAction(
            "Manage",
            new
            {
                id = 1,
                size,
                userName,
                groupId,
                siteId,
                statusId
            });
    }

    #endregion Manage

    #region Assign Site

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.AssignSiteToUser, Policy = Constant.Policy.Read)]
    public ActionResult AssignSite(
        int id,
        string returnUserName = EmptyString,
        int returnSiteId = 0,
        int returnGroupId = 0,
        int returnPageIndex = 1)
    {
        List<Site> siteList;
        var editableSiteList = new List<Site>();

        if (UserIsPortalAdmin)
        {
            siteList = siteService.GetAll(Environment, true).ToList();
            ViewFlags.HasAssignSiteReadRight = true;
            ViewFlags.HasAssignSiteRight = true;
        }
        else
        {
            siteList = siteService.GetByUserAndFunctionKeyIncludeGroup(
                NapSession.UserId,
                FunctionKeys.AssignSiteToUser,
                Environment).ToList();

            var assignSitePolicy = userService.GetUserSiteFunctionKeyPolicy(
                NapSession.UserId,
                SiteId,
                FunctionKeys.AssignSiteToUser);

            ViewFlags.HasAssignSiteReadRight = HasRight(FunctionKeys.AssignSiteToUser, AdminConstant.Policy.Read);
            ViewFlags.HasAssignSiteRight = assignSitePolicy >= (int)AdminConstant.Policy.Host;

            if (assignSitePolicy >= (int)AdminConstant.Policy.Host)
            {
                editableSiteList = siteService.GetByUserAndFunctionKeyIncludeGroup(
                    NapSession.UserId,
                    FunctionKeys.AssignSiteToUser,
                    Environment,
                    policy: AdminConstant.Policy.Host).ToList();
            }
        }

        var currentUserSiteLevelList = siteService.GetSiteLevelByUser(NapSession.UserId, Environment).ToList();
        var siteLevelList = siteService.GetSiteLevelByUser(id, Environment).ToList();

        var assignSiteVM = new AssignSiteVM
        {
            ReturnData = new ReturnData
            {
                ReturnUserName = returnUserName,
                ReturnSiteId = returnSiteId,
                ReturnGroupId = returnGroupId,
                ReturnPageIndex = returnPageIndex
            },
            SiteList = siteList,
            CurrentUserSiteLevelList = currentUserSiteLevelList,
            SiteLevelList = siteLevelList,
            EditableSiteList = editableSiteList,
            CurrentSessionLevel = NapSession.Level,
            UserId = id,
            UserName = userService.GetById(id).Username,
            IsSelfEdit = NapSession.UserId == id,
            CurrentSiteId = SiteId,
            IsPortalAdmin = UserIsPortalAdmin
        };

        return View(assignSiteVM);
    }

    #endregion Assign Site

    #region Get Available Sites for User

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.AssignSiteToUser, Policy = Constant.Policy.Host)]
    public ActionResult GetAvailableSitesForUser(
        int userId,
        int level,
        string username,
        string returnUserName = EmptyString,
        int returnSiteId = 0,
        int returnGroupId = 0,
        int returnPageIndex = 1)
    {
        var availableSites = new List<Site>();
        var hostRightSites = new List<Site>();
        var currentUserSiteLevelList = new List<SiteLevel>();

        if (NapSession.UserId != userId)
        {
            availableSites = siteService.GetAvailableSitesForUser(userId, Environment).ToList();
            currentUserSiteLevelList = siteService.GetSiteLevelByUser(NapSession.UserId, Environment, true).ToList();
            var isHasAssignRight = HasRight(FunctionKeys.AssignSiteToUser, AdminConstant.Policy.Host);

            if (isHasAssignRight)
            {
                hostRightSites = siteService.GetByUserAndFunctionKeyIncludeGroup(
                    NapSession.UserId,
                    FunctionKeys.AssignSiteToUser,
                    Environment,
                    policy: AdminConstant.Policy.Host).ToList();
            }
        }

        var assignNewSiteVM = new AssignNewSiteVM
        {
            ReturnData = new ReturnData
            {
                ReturnUserName = returnUserName,
                ReturnSiteId = returnSiteId,
                ReturnGroupId = returnGroupId,
                ReturnPageIndex = returnPageIndex
            },
            AvailableSitesForUser = availableSites,
            CurrentUserSiteLevelList = currentUserSiteLevelList,
            HostRightSites = hostRightSites,
            UserName = username,
            UserId = userId,
            Level = level
        };

        return View("~/Views/User/AssignNewSite.cshtml", assignNewSiteVM);
    }

    #endregion Get Available Sites for User

    #region User Info

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Read)]
    public ActionResult UserInfo(int id)
    {
        return View(userService.GetById(id));
    }

    #endregion User Info

    #region Add Existing User

    [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Host)]
    public ActionResult AddExisting(
        int page = 1,
        int size = 20,
        string userName = EmptyString,
        string returnUserName = EmptyString,
        int returnSiteId = 0,
        int returnGroupId = 0,
        int returnPageIndex = 0)
    {
        var addExistingUserVM = new AddExistingUserVM
        {
            ReturnData = new ReturnData
            {
                ReturnUserName = returnUserName,
                ReturnGroupId = returnGroupId,
                ReturnSiteId = returnSiteId,
                ReturnPageIndex = returnPageIndex
            }
        };

        var criteria = new SearchByUsernameUserCriteria
        {
            Username = userName,
            PageIndex = page,
            PageSize = size,
            TotalRows = 0
        };

        PagerList<User> userList = null;

        if (criteria.Username.Length > 0)
        {
            var users = userService.SearchByUsername(criteria);

            userList = new PagerList<User>(criteria.PageIndex, criteria.PageSize, criteria.TotalRows, users);

            ViewBag.Groups = groupService.GetAllSelectListItem();
            ViewBag.UserName = criteria.Username;

            if (Request.IsAjaxRequest())
            {
                addExistingUserVM.UserList = userList;
                return PartialView(addExistingUserVM);
            }
        }

        addExistingUserVM.UserList = userList;
        return View(addExistingUserVM);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Host)]
    public ActionResult AddExisting(AddExistingUserVM viewModel)
    {
        return RedirectToAction(
            "AddExisting",
            new
            {
                id = 1,
                size = viewModel.PageSize,
                userName = viewModel.UserName,
                returnUserName = viewModel.ReturnData.ReturnUserName,
                returnSiteId = viewModel.ReturnData.ReturnSiteId,
                returnGroupId = viewModel.ReturnData.ReturnGroupId,
                returnPageIndex = viewModel.ReturnData.ReturnPageIndex
            });
    }

    #endregion Add Existing User

    #region Add New User

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Host)]
    public ActionResult AddNew(
        string returnUserName = EmptyString,
        int returnSiteId = 0,
        int returnGroupId = 0,
        int returnPageIndex = 1)
    {
        var returnData = new ReturnData
        {
            ReturnUserName = returnUserName,
            ReturnSiteId = returnSiteId,
            ReturnGroupId = returnGroupId,
            ReturnPageIndex = returnPageIndex
        };

        ViewBag.Groups = groupService.GetAllSelectListItem();

        return View(new AddNewUserVM
        {
            ReturnData = returnData,
            Criteria = new InsertCustCriteria()
        });
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Host)]
    public ActionResult AddNew(AddNewUserVM addNewUserVM)
    {
        var criteria = addNewUserVM.Criteria;

        ViewBag.Groups = groupService.GetAllSelectListItem(selectedGroupId: criteria.GroupId);

        if (ModelState.IsValid)
        {
            if (userService.CheckUsernameExist(criteria.Username))
            {
                ModelState.AddModelError("Username", string.Format(ViewResources.DuplicateMsg, criteria.Username));
                return View(addNewUserVM);
            }

            criteria.FirstName ??= string.Empty;
            criteria.LastName ??= string.Empty;
            criteria.Email ??= string.Empty;

            var result = userService.Insert(criteria);

            if (result is { IsSuccessful: true })
            {
                ViewBag.Success = true;
                ViewBag.UserId = criteria.CustId;
            }
            else
            {
                ModelState.AddModelError("Username", result.Message);
            }
        }

        addNewUserVM.Criteria = criteria;

        return View(addNewUserVM);
    }

    #endregion Add New User

    #region Edit User

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Host)]
    public ActionResult Edit(
        int id,
        string returnUserName = EmptyString,
        int returnSiteId = 0,
        int returnGroupId = 0,
        int returnPageIndex = 1)
    {
        var criteria = new UpdateCustCriteria();

        var user = userService.GetById(id);

        if (user == null)
        {
            return RedirectToAction("Manage");
        }

        criteria.CustId = user.Id;
        criteria.FirstName = user.FirstName;
        criteria.LastName = user.LastName;
        criteria.Email = user.Email;
        criteria.Username = user.Username;
        criteria.GroupId = user.GroupId;

        ViewBag.Groups = user.GroupId == 0 ?
            groupService.GetAllSelectListItem() :
            groupService.GetAllSelectListItem(selectedGroupId: user.GroupId);

        return View(new EditUserVM
        {
            ReturnData = new ReturnData
            {
                ReturnUserName = returnUserName,
                ReturnSiteId = returnSiteId,
                ReturnGroupId = returnGroupId
            },
            Criteria = criteria
        });
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Host)]
    public ActionResult Edit(EditUserVM editUserVM)
    {
        var criteria = editUserVM.Criteria;

        if (ModelState.IsValid)
        {
            criteria.FirstName ??= string.Empty;
            criteria.LastName ??= string.Empty;
            criteria.Email ??= string.Empty;

            var result = userService.InsertOrUpdate(criteria);

            if (result is { IsSuccessful: true })
            {
                ViewBag.Success = true;
            }
        }

        ViewBag.Groups = groupService.GetAllSelectListItem(selectedGroupId: criteria.GroupId);

        editUserVM.Criteria = criteria;
        return View(editUserVM);
    }

    #endregion Edit User

    #region Reset Password

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Host)]
    public ActionResult ResetPassword(
        int id,
        string returnUserName = EmptyString,
        int returnSiteId = 0,
        int returnGroupId = 0,
        int returnPageIndex = 1)
    {
        var user = userService.GetById(id);

        if (user == null)
        {
            return RedirectToAction("Manage");
        }

        var resetPasswordVM = new ResetPasswordVM
        {
            ReturnData = new ReturnData
            {
                ReturnUserName = returnUserName,
                ReturnSiteId = returnSiteId,
                ReturnGroupId = returnGroupId,
                ReturnPageIndex = returnPageIndex
            },
            Criteria = new ResetPasswordCustCriteria
            {
                CustId = user.Id,
                Username = user.Username,
                UserPwd = string.Empty,
                ConfirmUserPwd = string.Empty
            }
        };

        return View(resetPasswordVM);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Host)]
    public ActionResult ResetPassword(ResetPasswordVM resetPasswordVM)
    {
        ModelState.Clear();

        var criteria = resetPasswordVM.Criteria;

        var user = userService.GetById(criteria.CustId);

        if (user == null)
        {
            return RedirectToAction("Manage");
        }

        var result = userService.InsertOrUpdate(criteria);

        if (result is { IsSuccessful: true })
        {
            ViewBag.Success = true;
        }

        resetPasswordVM.Criteria = criteria;

        return View(resetPasswordVM);
    }

    #endregion Reset Password

    #region Assign Permission

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToUser, Policy = Constant.Policy.Read)]
    public ActionResult AssignPermission(
        int id,
        int siteId = 0,
        string returnUserName = EmptyString,
        int returnPageIndex = 1,
        int returnSiteId = 0,
        int returnGroupId = 0)
    {
        var assignPermissionVM = new AssignPermissionVM();
        var siteIdValue = siteId == 0 ? SiteId : siteId;

        var user = userService.GetById(id);
        var site = siteService.GetById(siteIdValue);

        if (ReferenceEquals(user, null) || ReferenceEquals(site, null))
        {
            return NotFound();
        }

        var criteria = new AssignFunctionUserCriteria
        {
            UserId = id,
            Username = user.Username,
            SiteId = siteIdValue
        };

        if (user.GroupId > 0)
        {
            assignPermissionVM.GroupFuncPolicyList =
                functionService.GetByGroupAndSite(user.GroupId, criteria.SiteId, Environment).ToList();
        }

        ViewFlags.HasAssignFunctionRight = HasRightOnSite(criteria.SiteId, FunctionKeys.AssignFunctionToUser, AdminConstant.Policy.Host)
                                           && id != NapSession.UserId;

        assignPermissionVM.Sites = NapSession.Level == (int)AdminConstant.Level.PortalAdmin ?
            siteService.GetAllSelectListItem(Environment, true, criteria.SiteId).ToList() :
            siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                NapSession.UserId,
                FunctionKeys.AssignFunctionToUser,
                Environment,
                selectedSiteId: criteria.SiteId).ToList();

        assignPermissionVM.FunctionList = functionService.GetBySiteInHierarchy(criteria.SiteId, Environment, true).ToList();
        assignPermissionVM.UserFuncPolicyList =
            functionService.GetByUserAndSite(criteria.UserId, criteria.SiteId, Environment).ToList();
        assignPermissionVM.CurrentUserFuncPolicyList =
            functionService.GetByUserAndSiteIncludeGroup(NapSession.UserId, criteria.SiteId, Environment).ToList();
        assignPermissionVM.AdminLevel = NapSession.Level;
        assignPermissionVM.UserLevel = userService.GetUserSiteLevel(user.Id, site.Id);

        assignPermissionVM.Criteria = criteria;
        assignPermissionVM.ReturnData = new ReturnData
        {
            ReturnUserName = returnUserName,
            ReturnSiteId = returnSiteId != 0 ? returnSiteId : siteIdValue,
            ReturnGroupId = returnGroupId,
            ReturnPageIndex = returnPageIndex
        };

        return View(assignPermissionVM);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToUser, Policy = Constant.Policy.Host)]
    public ActionResult AssignPermission(AssignPermissionVM assignPermissionVM)
    {
        var criteria = assignPermissionVM.Criteria;

        var user = userService.GetById(criteria.UserId);
        var site = siteService.GetById(criteria.SiteId);

        if (user == null || site == null)
        {
            return RedirectToAction("Manage");
        }

        var policy = userService.GetUserSiteFunctionKeyPolicy(
            NapSession.UserId,
            criteria.SiteId,
            FunctionKeys.AssignFunctionToUser);

        if (policy < (int)AdminConstant.Policy.Host)
        {
            return RedirectToAction("Unauthorize", "Common");
        }

        ViewFlags.HasAssignFunctionRight = true;

        criteria.Username = user.Username;
        criteria.ModifiedBy = NapSession.UserId;
        criteria.ModifiedDate = DateTime.Now;

        var listFunc =
            functionService.GetBySiteInHierarchy(criteria.SiteId, Environment, true).ToList();

        var listUserFuncPolicy =
            functionService.GetByUserAndSite(criteria.UserId, criteria.SiteId, Environment)
                .Where(f => f.SiteId == criteria.SiteId).ToList();

        var listUserFuncRemove = new List<FunctionPolicy>();
        listUserFuncRemove.AddRange(listUserFuncPolicy);

        var listCurrentUserFuncPolicy =
            functionService.GetByUserAndSiteIncludeGroup(NapSession.UserId, criteria.SiteId, Environment).ToList();

        if (!criteria.FuncHost.IsNullOrDBNull())
        {
            foreach (var itemFunc in criteria.FuncHost)
            {
                var tempFuncHost = listFunc.Find(f => f.Id == itemFunc);

                if (tempFuncHost.IsNullOrDBNull())
                {
                    var errorMessage = $"tempFuncHost is null. listFunc={JsonConvert.SerializeObject(listFunc)}. itemFunc = {itemFunc}";
                    throw new ArgumentNullException(errorMessage);
                }

                listUserFuncRemove.Remove(f => f.Id == itemFunc);
            }
        }

        if (!criteria.FuncRead.IsNullOrDBNull())
        {
            foreach (var itemFunc in criteria.FuncRead)
            {
                listUserFuncRemove.Remove(f => f.Id == itemFunc);
            }
        }

        listUserFuncRemove = listUserFuncRemove.Where(f => f.Policy > 0).ToList();
        foreach (var itemFunc in listUserFuncRemove)
        {
            var tempFunc = listCurrentUserFuncPolicy
                .Find(f =>
                    f.Id == itemFunc.Id
                    && f.Policy >= itemFunc.Policy);

            if (tempFunc.IsNullOrDBNull())
            {
                var errorMessage = $"In order to assign this function to new user, current logging user must have higher permission on this function."
                                   + $"<br/>Technical information: tempFunc is null."
                                   + $"<br/> listCurrentUserFuncPolicy==== {JsonConvert.SerializeObject(listCurrentUserFuncPolicy, Formatting.Indented)} ====."
                                   + $"<br/> itemFunc ==== {JsonConvert.SerializeObject(itemFunc, Formatting.Indented)} ===="
                                   + $"<br/> listUserFuncPolicy ==== {JsonConvert.SerializeObject(listUserFuncPolicy, Formatting.Indented)} ===="
                                   + $"<br/> listUserFuncRemove ==== {JsonConvert.SerializeObject(listUserFuncRemove, Formatting.Indented)} ====";
                throw new ArgumentNullException(errorMessage);
            }
        }

        var result = userService.AssignFunction(criteria);

        if (result is { IsSuccessful: true })
        {
            ViewBag.Success = true;

            listUserFuncPolicy =
                functionService.GetByUserAndSite(criteria.UserId, criteria.SiteId, Environment).ToList();
        }

        if (user.GroupId > 0)
        {
            assignPermissionVM.GroupFuncPolicyList =
                functionService.GetByGroupAndSite(user.GroupId, criteria.SiteId, Environment).ToList();
        }

        assignPermissionVM.Sites = NapSession.Level == (int)AdminConstant.Level.PortalAdmin ?
            siteService.GetAllSelectListItem(Environment, true, criteria.SiteId).ToList() :
            siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                NapSession.UserId,
                FunctionKeys.AssignFunctionToUser,
                Environment,
                selectedSiteId: criteria.SiteId).ToList();

        assignPermissionVM.FunctionList = listFunc;
        assignPermissionVM.UserFuncPolicyList = listUserFuncPolicy;
        assignPermissionVM.CurrentUserFuncPolicyList = listCurrentUserFuncPolicy;
        assignPermissionVM.AdminLevel = NapSession.Level;
        assignPermissionVM.UserLevel = userService.GetUserSiteLevel(user.Id, site.Id);

        assignPermissionVM.Criteria = criteria;

        return View(assignPermissionVM);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToUser, Policy = Constant.Policy.Host)]
    public ActionResult UnassignPermission(int userId, int functionId)
    {

        var user = userService.GetById(userId);
        var function = functionService.GetById(functionId);
        var listUserFuncPolicy = functionService.GetByUserAndSite(userId, function.SiteId, Environment).ToList();

        var criteria = new AssignFunctionUserCriteria
        {
            FuncHost = listUserFuncPolicy.Where(p => p.Policy == 2 && p.Id != functionId).Select(p => (int)p.Id).ToArray(),
            FuncRead = listUserFuncPolicy.Where(p => p.Policy == 1 && p.Id != functionId).Select(p => (int)p.Id).ToArray(),
            ModifiedBy = NapSession.UserId,
            ModifiedDate = DateTime.Now,
            SiteId = function.SiteId,
            Username = user.Username,
            UserId = userId
        };

        var result = userService.AssignFunction(criteria);

        return Json(result);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.AssignSiteToUser, Policy = Constant.Policy.Host)]
    public string AssignSiteToUser(int userId, int siteId, int level, bool isActive)
    {
        var criteria = new AssignSiteUserCriteria
        {
            CreatedBy = NapSession.UserId,
            CreatedDate = DateTime.Now,
            UserId = userId,
            SiteId = siteId,
            Level = level,
            IsActive = isActive,
            IsNeverExpired = true
        };

        var result = userService.AssignSite(criteria);

        if (result is not { IsSuccessful: true })
        {
            return result.Message;
        }

        return siteId == NapAdminSettings.OWPSiteID
            ? userService.SyncUserToOWP(userId)
            : AdminConstant.Ok;

    }

    [HttpGet]
    public string SyncUserToOWP(int userId)
    {
        userService.SyncUserToOWP(userId);
        return "OK";
    }

    #endregion Assign Permission

    #region Update Site To User

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.AssignSiteToUser, Policy = Constant.Policy.Host)]
    public string UpdateSiteToUser(int userId, int siteId, int level, bool isActive, DateTime expiredDate, bool isNeverExpired)
    {
        var canUpdateSiteToUser = CheckPermissionToUpdateUser(userId, siteId, level);

        if (canUpdateSiteToUser)
        {
            var criteria = new UpdateSiteToUserCriteria
            {
                ModifiedBy = NapSession.UserId,
                ModifiedDate = DateTime.Now,
                UserId = userId,
                SiteId = siteId,
                Level = level,
                IsActive = isActive,
                ExpiredDate = expiredDate,
                IsNeverExpired = isNeverExpired
            };

            var result = userService.UpdateSite(criteria);

            if (result is { IsSuccessful: true })
            {
                return AdminConstant.Ok;
            }
        }

        return AdminConstant.Error;
    }

    #endregion Update Site To User

    #region Profile

    [HttpGet]
    public ActionResult UserProfile()
    {
        var criteria = new UpdateCustCriteria();
        var user = userService.GetById(NapSession.UserId);

        criteria.CustId = user.Id;
        criteria.Username = user.Username;
        criteria.FirstName = user.FirstName;
        criteria.LastName = user.LastName;
        criteria.Email = user.Email;
        criteria.GroupId = user.GroupId;

        return View(criteria);
    }

    [HttpPost]
    public ActionResult UserProfile(UpdateCustCriteria criteria)
    {
        if (ModelState.IsValid)
        {
            criteria.CustId = NapSession.UserId;
            criteria.Username = NapSession.UserName;
            criteria.FirstName ??= string.Empty;
            criteria.LastName ??= string.Empty;
            criteria.Email ??= string.Empty;

            var result = userService.InsertOrUpdate(criteria);

            if (result is { IsSuccessful: true })
            {
                ViewBag.Success = true;
            }
        }

        return View(criteria);
    }

    #endregion Profile

    #region Assign Site/Function To User

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.AssignSiteToUser, Policy = Constant.Policy.Read)]
    [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToUser, Policy = Constant.Policy.Read)]
    public ActionResult AssignSiteFunction(
        int id,
        string returnUserName = EmptyString,
        int returnSiteId = 0,
        int returnGroupId = 0,
        int returnPageIndex = 1)
    {
        var loginUserAssignSitePolicy = userService.GetUserSiteFunctionKeyPolicy(
            NapSession.UserId,
            SiteId,
            FunctionKeys.AssignSiteToUser);

        var loginUserAssignFunctionPolicy = userService.GetUserSiteFunctionKeyPolicy(
            NapSession.UserId,
            SiteId,
            FunctionKeys.AssignFunctionToUser);

        return View(new AssignSiteFunctionViewModel
        {
            LoginUserId = NapSession.UserId,
            UserName = userService.GetById(id).Username,
            UserId = id,
            LoginUserIsPortalAdmin = UserIsPortalAdmin,
            LoginUserHasAssignSitePermission = loginUserAssignSitePolicy >= (int)AdminConstant.Policy.Host,
            LoginUserHasAssignSiteReadPermission = loginUserAssignSitePolicy == (int)AdminConstant.Policy.Read,
            LoginUserAssignFunctionPermission = loginUserAssignFunctionPolicy,
            IsSelfEdit = NapSession.UserId == id,
            CurrentSiteId = SiteId,
            ReturnData = new ReturnData
            {
                ReturnUserName = returnUserName,
                ReturnGroupId = returnGroupId,
                ReturnPageIndex = returnPageIndex,
                ReturnSiteId = returnSiteId
            }
        });
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToUser, Policy = Constant.Policy.Host)]
    public ActionResult AssignFunction([FromBody] List<AssignFunctionUserCriteria> payload)
    {
        foreach (var data in payload)
        {
            var user = userService.GetById(data.UserId);
            var site = siteService.GetById(data.SiteId);

            if (user == null || site == null)
            {
                return RedirectToAction("Manage");
            }

            data.Username = user.Username;
            data.ModifiedBy = NapSession.UserId;
            data.ModifiedDate = DateTime.Now;

            var result = userService.AssignFunction(data);

            if (result is { IsSuccessful: true })
            {
                return Json(AdminConstant.Ok);
            }
        }

        return Json(AdminConstant.Error);
    }

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.AssignSiteToUser, Policy = Constant.Policy.Read)]
    public JsonResult GetAllUserSites(int userId)
    {
        IEnumerable<Site> siteList = UserIsPortalAdmin || NapSession.UserId == userId ?
            siteService.GetAll(Environment, true).ToList() :
            siteService.GetByUserAndFunctionKeyIncludeGroup(
                NapSession.UserId,
                FunctionKeys.AssignSiteToUser,
                Environment).ToList();

        var siteLevelList = siteService.GetSiteLevelByUser(userId, Environment);
        var userSiteList = siteList.Where(s => siteLevelList.Any(sl => sl.SiteId == s.Id)).ToList();

        userSiteList = userSiteList.Union(GetUserUnassignedSites(userId)).ToList();
        GetUserHasFunctionPermissionSites(userSiteList);

        return Json(userSiteList);
    }

    private void GetUserHasFunctionPermissionSites(List<Site> userSiteList)
    {
        var loginUserHasFunctionPermissionSites = siteService.GetByUserAndFunctionKeyIncludeGroup(
            NapSession.UserId,
            FunctionKeys.AssignFunctionToUser,
            Environment);

        foreach (var site in loginUserHasFunctionPermissionSites)
        {
            if (userSiteList.TrueForAll(s => s.Id != site.Id))
            {
                userSiteList.Add(site);
            }
        }
    }

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToUser, Policy = Constant.Policy.Read)]
    public JsonResult GetFunctions(int siteId, int userId)
    {
        var user = userService.GetById(userId);
        var groupFunctionPolicies = new List<FunctionPolicy>();

        if (user.GroupId > 0)
        {
            groupFunctionPolicies = functionService.GetByGroupAndSite(user.GroupId, siteId, Environment).ToList();
        }

        var functions = functionService.GetBySiteInHierarchy(siteId, Environment, true).ToList();
        var functionPolicies = functionService.GetByUserAndSite(userId, siteId, Environment).ToList();
        var loginUserFunctionPolicies = functionService.GetByUserAndSiteIncludeGroup(NapSession.UserId, siteId, Environment).ToList();
        var loginUserAssignFunctionPermission = userService.GetUserSiteFunctionKeyPolicy(
            NapSession.UserId,
            siteId,
            FunctionKeys.AssignFunctionToUser);

        var loginUserAssignSitePermission = userService.GetUserSiteFunctionKeyPolicy(
            NapSession.UserId,
            siteId,
            FunctionKeys.AssignSiteToUser);

        return Json(
            new
            {
                functions,
                functionPolicies,
                groupFunctionPolicies,
                loginUserFunctionPolicies,
                loginUserAssignFunctionPermission,
                loginUserAssignSitePermission
            });
    }

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.AssignSiteToUser, Policy = Constant.Policy.Read)]
    public ActionResult GetUserSiteLevels(int userId)
    {
        var listSiteLive = siteService.GetSiteLevelByUser(userId, Environment);

        return Json(listSiteLive);
    }

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.AssignSiteToUser, Policy = Constant.Policy.Read)]
    public ActionResult GetLoginUserHasAssignSiteHostPermissionSites()
    {
        var assignSitePolicy = userService.GetUserSiteFunctionKeyPolicy(
            NapSession.UserId,
            SiteId,
            FunctionKeys.AssignSiteToUser);

        var editableSites = new List<Site>();

        if (assignSitePolicy >= (int)AdminConstant.Policy.Host)
        {
            editableSites = siteService.GetByUserAndFunctionKeyIncludeGroup(
                NapSession.UserId,
                FunctionKeys.AssignSiteToUser,
                Environment,
                policy: AdminConstant.Policy.Host).ToList();
        }

        return Json(editableSites);
    }

    #endregion Assign Site/Function To User

    #region Ajax

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Read)]
    public ActionResult UserList(int groupId, int siteId = 0)
    {
        if (Request.IsAjaxRequest())
        {
            var userList = userService.GetByGroupAndSite(groupId, siteId);

            return View(userList);
        }

        return new EmptyResult();
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Read)]
    public ActionResult GetUserSite(int userId)
    {
        if (Request.IsAjaxRequest())
        {
            var listSiteLive = siteService.GetSiteLevelByUser(userId, Environment);

            return Json(listSiteLive);
        }

        return Json(null);
    }

    [HttpPost]
    public ActionResult GetUserFunctionPolicy(int userId)
    {
        if (Request.IsAjaxRequest())
        {
            var listFunctionPolicy = functionService.GetByUser(userId, Environment);

            return Json(listFunctionPolicy);
        }

        return Json(null);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Host)]
    public string AddUserToGroup(int userId, int groupId)
    {
        if (Request.IsAjaxRequest() && userId > 0 && groupId > 0)
        {
            var criteria = new InsertUserCriteria
            {
                UserId = userId,
                GroupId = groupId,
                CreatedBy = NapSession.UserId,
                CreatedDate = DateTime.Now
            };

            var user = userService.GetById(userId);

            if (user == null)
            {
                var result = userService.InsertOrUpdate(criteria);

                if (result is { IsSuccessful: true })
                {
                    return AdminConstant.Ok;
                }
            }
        }

        return AdminConstant.Error;
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Read)]
    public ActionResult GetUserByGroup(int groupId)
    {
        if (Request.IsAjaxRequest())
        {
            var listUser = userService.GetByGroupAndSite(groupId);

            return Json(listUser);
        }

        return Json(null);
    }

    #endregion Ajax

    #region Private method


    private bool CheckPermissionToUpdateUser(int userId, int siteId, int level)
    {
        if (level == (int)AdminConstant.Level.PortalAdmin)
        {
            return false;
        }

        if (NapSession.UserId != userId)
        {
            if (UserIsPortalAdmin)
            {
                return true;
            }

            if (!HasRight(FunctionKeys.AssignSiteToUser, AdminConstant.Policy.Host))
            {
                return false;
            }

            var editableSiteList = siteService.GetByUserAndFunctionKeyIncludeGroup(
                NapSession.UserId,
                FunctionKeys.AssignSiteToUser,
                Environment,
                policy: AdminConstant.Policy.Host).ToList();

            if (editableSiteList.TrueForAll(s => s.Id != siteId))
            {
                return false;
            }

            var currentUserSiteLevelList = siteService.GetSiteLevelByUser(NapSession.UserId, Environment).ToList();

            if (currentUserSiteLevelList.TrueForAll(s => s.SiteId != siteId))
            {
                return false;
            }

            var currentUserSiteLevel = currentUserSiteLevelList.Find(s => s.SiteId == siteId).Level;

            var siteLevelList = siteService.GetSiteLevelByUser(userId, Environment).ToList();

            if (siteLevelList.TrueForAll(s => s.SiteId != siteId))
            {
                return false;
            }

            var selectedUserSiteLevel = siteLevelList.Find(s => s.SiteId == siteId).Level;

            return currentUserSiteLevel == (int)AdminConstant.Level.User ?
                selectedUserSiteLevel == (int)AdminConstant.Level.User && level == (int)AdminConstant.Level.User :
                currentUserSiteLevel > selectedUserSiteLevel && currentUserSiteLevel > level;
        }

        return SiteId != siteId && UserIsPortalAdmin;
    }


    private IEnumerable<Site> GetUserUnassignedSites(int userId)
    {
        var loginUserSiteLevelList = siteService.GetSiteLevelByUser(NapSession.UserId, Environment).ToList();
        var loginUserHasAssignSiteHostRightSites = siteService.GetByUserAndFunctionKeyIncludeGroup(
            NapSession.UserId,
            FunctionKeys.AssignSiteToUser,
            Environment,
            policy: AdminConstant.Policy.Host).ToList();

        var availableSites = siteService.GetAvailableSitesForUser(userId, Environment).ToList();
        var unassignedSite = new List<Site>();

        foreach (var site in availableSites)
        {
            var loginUserSiteLevel = loginUserSiteLevelList.Find(s => s.SiteId == site.Id);

            if (loginUserSiteLevel is { Level: >= (int)AdminConstant.Level.User } &&
                loginUserHasAssignSiteHostRightSites.Exists(s => s.Id == site.Id))
            {
                unassignedSite.Add(site);
            }
        }

        return unassignedSite;
    }

    #endregion Private method

    #region SetupOTP

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.SetupOTP, Policy = Constant.Policy.Read)]
    public ActionResult SetupOTP(int page = 1, int size = 20, string username = "", string system = "", int status = 1)
    {
        var trimmedUsername = username.Trim();

        ViewBag.Username = trimmedUsername;

        ViewBag.HasEditRight = HasRight(FunctionKeys.SetupOTP, AdminConstant.Policy.Host);

        var criteria = new SetupOTPListCriteria
        {
            PageIndex = page,
            PageSize = size,
            Username = trimmedUsername,
            System = system,
            Status = status,
            TotalRows = 0
        };

        var repository = new DynamicRepository();

        var users = repository.Fetch<SetupOTPItem>(criteria)
            .AsQueryable();
        return View(new PagerList<SetupOTPItem>(page, size, criteria.TotalRows, users));
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.SetupOTP, Policy = Constant.Policy.Host)]
    public string SetupOTPUpdate(int custId, string system, string secretKey, bool ignoreOTP)
    {
        var command = new SetupOTPUpdateCommand
        {
            CustId = custId,
            SecretKey = secretKey,
            System = system,
            IgnoreOTP = ignoreOTP,
            LoginIP = loggedUserContext.GetIp(),
            Modifier = NapSession.UserId
        };

        var repository = new DynamicRepository();
        repository.Execute(command);

        return string.IsNullOrEmpty(command.ErrMsg) ? string.Empty : command.ErrMsg;
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.SetupOTP, Policy = Constant.Policy.Host)]
    public ActionResult SetupOTPGenerateKey(int custId, string system, bool ignoreOTP)
    {
        var otpService = new OtpServiceImpl();
        var secretKey = otpService.GenerateSecretKey();

        var command = new SetupOTPUpdateCommand
        {
            CustId = custId,
            SecretKey = secretKey,
            System = system,
            IgnoreOTP = ignoreOTP,
            LoginIP = loggedUserContext.GetIp(),
            Modifier = NapSession.UserId
        };

        var repository = new DynamicRepository();
        repository.Execute(command);

        var errMsg = string.IsNullOrEmpty(command.ErrMsg) ? string.Empty : command.ErrMsg;

        var result = new
        {
            SecretKey = secretKey,
            ErrMsg = errMsg
        };

        return Json(result);
    }
    #endregion
}
