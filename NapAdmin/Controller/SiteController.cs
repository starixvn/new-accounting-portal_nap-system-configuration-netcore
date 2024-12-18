namespace NapAdmin.Controller;

using System;
using System.Security.Cryptography;
using Fanex.NAP.Client;
using Fanex.NAP.Client.Attributes;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Business;
using Microsoft.AspNetCore.Mvc;
using NapAdminCore.Configuration;
using NapAdminCore.Criteria.Site;
using NapAdminCore.Models;
using NapAdminCore.Models.ValueObject;
using NapAdminCore.Service;
using NapAdminCore.Web;

[SessionAuthorize]
public class SiteController(ISiteService siteService,
        INapSession napSession,
        INapSettings napSettings,
        IUserService userService)
    : BaseController(napSession, napSettings, userService)
{
    #region Manage

    [NapAuth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Read)]
    public ActionResult Manage(GetAllSiteCriteria criteria)
    {
        ViewFlags.HasEditRight = HasRight(FunctionKeys.ManageSite, AdminConstant.Policy.Host);
        ViewFlags.HasManageUserRight = HasRight(FunctionKeys.ManageUser, AdminConstant.Policy.Read);

        ViewBag.Site = siteService.GetByUserAndFunctionKeyIncludeGroup(
            NapSession.UserId,
            FunctionKeys.ManageUser,
            Environment).Select(s => s.Id).ToList();
        ViewBag.SiteList = siteService.GetAllWithNumOfUsers(Environment).OrderByDescending(s => s.IsActive).ThenBy(s => s.Id).ToList();
        ViewBag.SiteUrlList = siteService.GetAllSiteUrl(Environment).ToList();

        return View(criteria);
    }

    #endregion Manage

    #region Add New

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
    public ActionResult AddNew()
    {
        var criteria = new InsertSiteCommand
        {
            Environment = Environment,
            Urls =
            [
                new SiteUrl
                {
                    IsActive = true,
                    IsDefault = true
                }
            ]
        };

        return View(criteria);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
    public ActionResult AddNew(InsertSiteCommand criteria)
    {
        if (!ModelState.IsValid)
        {
            return View(criteria);
        }

        var exists = !ReferenceEquals(criteria.Urls, null)
                     && criteria.Urls.Exists(itemUrl => !Uri.IsWellFormedUriString(itemUrl.Url, UriKind.Absolute));
        if (exists)
        {
            ModelState.AddModelError("SiteUrls", string.Format(ViewResources.InvalidURLValidationMsg, criteria.SiteUrls));
            return View(criteria);
        }

        criteria.SiteName = criteria.SiteName.Trim();
        var site = siteService.GetByName(criteria.SiteName);

        if (site != null)
        {
            ModelState.AddModelError("SiteName", string.Format(ViewResources.DuplicateMsg, criteria.SiteName));

            return View(criteria);
        }

        criteria.SiteKey = ConvertToKey(criteria.SiteName);
        site = siteService.GetByKey(criteria.SiteKey);

        if (site != null)
        {
            ModelState.AddModelError("SiteKey", string.Format(ViewResources.DuplicateMsg, criteria.SiteKey));

            return View(criteria);
        }

        criteria.Urls = criteria.Urls.Where(x => !string.IsNullOrWhiteSpace(x.Url)).ToList();

        if (criteria.Urls != null)
        {
            var distinctCount = criteria.Urls.Select(x => x.Url).Distinct().Count();

            if (distinctCount != criteria.Urls.Count)
            {
                ModelState.AddModelError("Urls", ViewResources.The_Urls_must_be_unique);

                return View(criteria);
            }
        }

        criteria.Environment = Environment;
        criteria.SecretAccessKey = GenerateAPPKey();

        var result = siteService.Insert(criteria);

        if (result is { IsSuccessful: true })
        {
            ViewBag.Success = true;
        }

        return View(criteria);
    }

    #endregion Add New

    #region Edit

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
    public ActionResult Edit(int id)
    {
        var site = siteService.GetById(id);

        if (site == null)
        {
            return RedirectToAction("NotFound", "Common");
        }

        var criteria = new UpdateSiteCommand
        {
            SiteId = site.Id,
            SiteName = site.Name,
            SiteKey = site.SiteKey,
            SecretAccessKey = site.SecretAccessKey,
            Description = site.Description,
            IsActive = site.IsActive,
            Environment = site.Environment
        };

        return View(criteria);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
    public ActionResult Edit(UpdateSiteCommand criteria)
    {
        if (ModelState.IsValid)
        {
            criteria.SiteName = criteria.SiteName.Trim();
            var site = siteService.GetByName(criteria.SiteName);

            if (site != null && site.Id != criteria.SiteId)
            {
                ModelState.AddModelError("SiteName", string.Format(ViewResources.DuplicateMsg, criteria.SiteName));

                return View(criteria);
            }

            if (!string.IsNullOrEmpty(criteria.SiteKey))
            {
                site = siteService.GetByKey(criteria.SiteKey);

                if (site != null && site.Id != criteria.SiteId)
                {
                    ModelState.AddModelError("SiteKey", string.Format(ViewResources.DuplicateMsg, criteria.SiteKey));
                    return View(criteria);
                }
            }

            var result = siteService.Update(criteria);

            if (result is { IsSuccessful: true })
            {
                ViewBag.Success = true;
            }
        }

        return View(criteria);
    }

    #endregion Edit

    #region SiteUrl

    [HttpGet]
    [NapAuth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Read)]
    public ActionResult EditUrl(int id)
    {
        var site = siteService.GetById(id);

        if (site == null)
        {
            return NotFound();
        }

        ViewBag.SiteId = site.Id;
        ViewBag.SiteName = site.Name;
        ViewBag.Urls = siteService.GetSiteUrlBySiteId(id, string.Empty).ToList();
        ViewFlags.SetEditRight(UserService.GetUserSiteFunctionKeyPolicy(
            NapSession.UserId,
            SiteId,
            FunctionKeys.ManageSite));

        return View();
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
    public ActionResult UpdateSiteUrl(int id, Uri url, bool isActive, string siteUrlEnvironment)
    {
        if (!Request.IsAjaxRequest())
        {
            return NotFound();
        }

        var siteUrl = siteService.GetSiteUrlById(id);

        if (siteUrl == null)
        {
            return NotFound();
        }

        var result = AdminConstant.Ok;

        if (url == null || !url.IsAbsoluteUri)
        {
            result = "invalid";
        }
        else
        {
            var altUrls = siteService.GetSiteUrlBySiteId(siteUrl.SiteId, siteUrl.Environment);

            if (altUrls.Any(x => x.SiteUrlId != id && x.Url == url.OriginalString))
            {
                result = "duplicate";
            }
            else
            {
                var criteria = new UpdateSiteUrlCommand
                {
                    SiteUrlId = id,
                    SiteId = siteUrl.SiteId,
                    Url = url.OriginalString,
                    Environment = siteUrlEnvironment,
                    IsActive = isActive,
                    IsDefault = siteUrl.IsDefault
                };

                var cmdResult = siteService.UpdateSiteUrl(criteria);

                if (cmdResult is not { IsSuccessful: true })
                {
                    result = "error";
                }
            }
        }

        return Json(result);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
    public ActionResult SetDefaultSiteUrl(int id, string siteUrlEnvironment)
    {
        if (Request.IsAjaxRequest())
        {
            var siteUrl = siteService.GetSiteUrlById(id);

            if (siteUrl != null)
            {
                var criteria = new UpdateDefaultSiteUrlCommand
                {
                    SiteUrlId = id,
                    SiteId = siteUrl.SiteId,
                    Environment = siteUrlEnvironment
                };

                var cmdResult = siteService.UpdateDefaultSiteUrl(criteria);

                return Json(cmdResult is not { IsSuccessful: true } ? "error" : AdminConstant.Ok);
            }
        }

        return NotFound();
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
    public ActionResult AddSiteUrl(int siteId, Uri url, bool isActive, string siteUrlEnvironment)
    {
        if (!Request.IsAjaxRequest())
        {
            return NotFound();
        }

        var site = siteService.GetById(siteId);

        if (site == null)
        {
            return NotFound();
        }
        string result;

        if (url == null || !url.IsAbsoluteUri)
        {
            result = "invalid";
        }
        else
        {
            var siteUrls = siteService.GetSiteUrlBySiteId(site.Id, Environment);

            if (siteUrls.Any(x => x.Url == url.OriginalString))
            {
                result = "duplicate";
            }
            else
            {
                var criteria = new InsertSiteUrlCommand
                {
                    SiteId = siteId,
                    Url = url.OriginalString,
                    Environment = siteUrlEnvironment,
                    IsActive = isActive,
                    IsDefault = false
                };

                var cmdResult = siteService.InsertSiteUrl(criteria);

                result = cmdResult is { IsSuccessful: true } ?
                    criteria.SiteUrlId.ToString() :
                    "error";
            }
        }

        return Json(result);
    }

    #endregion SiteUrl

    #region Activation

    [NapAuth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
    public ActionResult Activate(int id, bool isActive)
    {
        if (Request.IsAjaxRequest())
        {
            var result = siteService.UpdateStatus(new UpdateStatusSiteCommand
            {
                SiteId = id,
                IsActive = isActive
            });

            if (result is { IsSuccessful: true })
            {
                return Json(AdminConstant.Ok);
            }
        }

        return new EmptyResult();
    }

    #endregion Activation

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
    public JsonResult GetSiteKey(int siteId, string siteName)
    {
        var site = siteService.GetById(siteId);

        if (ReferenceEquals(site, null))
        {
            return Json(-1);
        }

        site = siteService.GetByName(siteName);

        if (site != null && site.Id != siteId)
        {
            return Json(0);
        }

        var siteKey = ConvertToKey(siteName);

        site = siteService.GetByKey(siteKey);

        if (site != null && site.Id != siteId)
        {
            return Json(-2);
        }

        return Json(siteKey);
    }

    [HttpPost]
    [NapAuth(FunctionKey = FunctionKeys.ManageSite, Policy = Constant.Policy.Host)]
    public JsonResult GetSecretAccessKey()
    {
        return Json(GenerateAPPKey());
    }

    private static string GenerateAPPKey()
    {
        using var cryptoProvider = new RNGCryptoServiceProvider();
        var secretKeyByteArray = new byte[32];

        cryptoProvider.GetBytes(secretKeyByteArray);
        var apiKey = Convert.ToBase64String(secretKeyByteArray);

        return apiKey;
    }
}
