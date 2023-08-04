namespace NapAdmin.Controller;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fanex.NAP.Client.Attributes;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NapAdminCore.Configuration;
using NapAdminCore.Criteria.Trace;
using NapAdminCore.Helpers;
using NapAdminCore.Models;
using NapAdminCore.Models.ValueObject;
using NapAdminCore.Service;
using NapAdminCore.Web;
using ViewModels.Trace;

public class TraceController : BaseController
{
    private readonly ITraceService _traceService;
    private readonly IFunctionService _functionService;
    private readonly ISiteService _siteService;


    public TraceController(ITraceService traceService,
        IFunctionService functionService,
        ISiteService siteService,
        INapSession napSession,
        INapSettings napSettings,
        IUserService userService)
        : base(napSession, napSettings, userService)
    {

        _traceService = traceService;
        _functionService = functionService;
        _siteService = siteService;
    }

    #region Log Exec Sp

    [NapAuth(FunctionKey = FunctionKeys.ExecSpLog)]
    [HttpGet]
    public ActionResult LogExecSp(
        int id = 1,
        int size = 20,
        string q = EmptyString,
        string username = EmptyString,
        DateTime? from = null,
        DateTime? to = null,
        int filterBy = 1)
    {
        var criteria = new GetAllLogCriteria
        {
            PageIndex = id,
            PageSize = size,
            Name = q ?? string.Empty,
            Username = username,
            From = from ?? DateTime.Now,
            To = to ?? DateTime.Now.AddDays(1),
            FilterType = filterBy,
            TotalRows = 0
        };

        var listLog = _traceService.SearchExecSp(criteria);
        ViewBag.ListLog = new PagerList<LogExecSp>(criteria.PageIndex, criteria.PageSize, criteria.TotalRows, listLog);

        return View(criteria);
    }

    [HttpPost]
    public ActionResult LogExecSp(GetAllLogCriteria criteria)
    {
        return RedirectToAction(
            "LogExecSp",
            new
            {
                id = criteria.PageIndex,
                size = criteria.PageSize,
                q = criteria.Name,
                username = criteria.Username,
                from = criteria.From.ToString("MMM dd,yyyy"),
                to = criteria.To.ToString("MMM dd,yyyy"),
                filterBy = criteria.FilterType
            });
    }

    #endregion Log Exec Sp

    #region Log User Login

    [HttpGet]
    public ActionResult LogUserLogin(
        int id = 1,
        int size = 20,
        string q = EmptyString,
        DateTime? from = null,
        DateTime? to = null,
        int siteId = 0)
    {
        var criteria = new GetAllLogUserLoginCriteria
        {
            PageIndex = id,
            PageSize = size,
            Name = q ?? string.Empty,
            From = from ?? DateTime.Now,
            To = to ?? DateTime.Now.AddDays(1),
            SiteId = siteId,
            TotalRows = 0
        };

        var listLog = _traceService.SearchUserLogin(criteria);

        var listSite = _siteService.GetAll().ToList();
        listSite.Add(new Site
        {
            Id = 0,
            Name = ViewResources.NAP_Hub
        });

        ViewBag.ListSite = listSite.Select(s => new SelectListItem
        {
            Text = s.Name,
            Value = s.Id.ToString()
        }).ToList();

        ViewBag.ListLog = new PagerList<LogUserLogin>(criteria.PageIndex, criteria.PageSize, criteria.TotalRows, listLog);

        return View(criteria);
    }

    [HttpPost]
    public ActionResult LogUserLogin(GetAllLogUserLoginCriteria criteria)
    {
        return RedirectToAction(
            "LogUserLogin",
            new
            {
                id = criteria.PageIndex,
                size = criteria.PageSize,
                q = criteria.Name,
                from = criteria.From.ToString("MMM dd,yyyy"),
                to = criteria.To.ToString("MMM dd,yyyy"),
                siteId = criteria.SiteId
            });
    }

    #endregion Log User Login

    #region Ajax

    [HttpPost]
    public ActionResult GetLogInsertDetail(int id)
    {
        if (Request.IsAjaxRequest())
        {
            var log = _traceService.GetById(id);

            return View(log);
        }

        return View(new LogExecSp());
    }

    #endregion Ajax

    #region View log

    public PartialViewResult NAPUserInsert(LogExecSp log)
    {
        if (log.ActionId == (int)LogAction.NAP_User_Insert)
        {
            var elementNew = XElement.Parse(log.Data);

            var listLogData = new List<LogData>();
            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Username,
                NewValue = elementNew.Attribute("Username").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Group_Name,
                NewValue = elementNew.Attribute("GroupName").Value
            });

            ViewBag.LogData = listLogData;
        }

        return PartialView("~/Views/Trace/_NAPLogInsert.cshtml");
    }

    public PartialViewResult NAPCustInsert(LogExecSp log)
    {
        var element = XElement.Parse(log.Data);

        if (log.ActionId == (int)LogAction.NAP_Cust_Insert)
        {
            var listLogData = new List<LogData>();
            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Username,
                NewValue = element.Attribute("Username").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Group_Name,
                NewValue = element.Attribute("GroupName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.First_Name,
                NewValue = element.Attribute("FirstName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Last_Name,
                NewValue = element.Attribute("LastName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Email,
                NewValue = element.Attribute("Email").Value
            });

            ViewBag.LogData = listLogData;
        }

        return PartialView("~/Views/Trace/_NAPLogInsert.cshtml");
    }

    public PartialViewResult NAPFunctionInsert(LogExecSp log)
    {
        var element = XElement.Parse(log.Data);

        if (log.ActionId == (int)LogAction.NAP_Function_Insert)
        {
            var listLogData = new List<LogData>();
            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Function_Name,
                NewValue = element.Attribute("FunctionName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Site_Name,
                NewValue = element.Attribute("SiteName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Function_Key,
                NewValue = element.Attribute("FunctionKey").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Description,
                NewValue = element.Attribute("Description").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Parent,
                NewValue = element.Attribute("ParentName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.CRUD,
                NewValue = element.Attribute("CRUD").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Display_Order,
                NewValue = element.Attribute("DisplayOrder").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Active,
                NewValue = element.Attribute("IsActive").Value == "1" ? "true" : "false"
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Environment,
                NewValue = element.Attribute("Environment").Value
            });

            ViewBag.LogData = listLogData;
        }

        return PartialView("~/Views/Trace/_NAPLogInsert.cshtml");
    }

    public PartialViewResult NAPGroupInsert(LogExecSp log)
    {
        var element = XElement.Parse(log.Data);

        if (log.ActionId == (int)LogAction.NAP_Group_Insert)
        {
            var listLogData = new List<LogData>();
            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Group_Name,
                NewValue = element.Attribute("GroupName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Description,
                NewValue = element.Attribute("Description").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Active,
                NewValue = element.Attribute("IsActive").Value == "1" ? "true" : "false"
            });

            ViewBag.LogData = listLogData;
        }

        return PartialView("~/Views/Trace/_NAPLogInsert.cshtml");
    }

    public PartialViewResult NAPMenuItemInsert(LogExecSp log)
    {
        var element = XElement.Parse(log.Data);

        if (log.ActionId == (int)LogAction.NAP_MenuItem_Insert)
        {
            var listLogData = new List<LogData>();
            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Menu_Item_Name,
                NewValue = element.Attribute("MenuItemName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Function_Name,
                NewValue = element.Attribute("FunctionName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Site_Name,
                NewValue = element.Attribute("SiteName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Description,
                NewValue = element.Attribute("Description").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Parent,
                NewValue = element.Attribute("ParentName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Policy,
                NewValue = element.Attribute("Policy").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.URL,
                NewValue = element.Attribute("URL").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Display_Order,
                NewValue = element.Attribute("DisplayOrder").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Active,
                NewValue = element.Attribute("IsActive").Value == "1" ? "true" : "false"
            });

            ViewBag.LogData = listLogData;
        }

        return PartialView("~/Views/Trace/_NAPLogInsert.cshtml");
    }

    public PartialViewResult NAPSiteInsertWithSiteUrl(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);

        if (log.ActionId == (int)LogAction.NAP_Site_InsertWithSiteURL)
        {
            var listLogData = new List<LogData>();
            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Site_Name,
                NewValue = elementNew.Attribute("SiteName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Site_Key,
                NewValue = elementNew.Attribute("SiteKey").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Secret_Access_Key,
                NewValue = elementNew.Attribute("SecretAccessKey").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Description,
                NewValue = elementNew.Attribute("Description").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Environment,
                NewValue = elementNew.Attribute("Environment").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Active,
                NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
            });

            var strUrl = elementNew.Attribute("Value").Value.DecodeXMLValue();

            var listSiteUrl = new List<SiteUrl>();
            var documentSiteUrl = XDocument.Parse(strUrl);
            var siteUrl = documentSiteUrl.Descendants("ITEM");

            foreach (var item in siteUrl)
            {
                var objSiteUrl = new SiteUrl
                {
                    Url = item.Attribute("URL").Value,
                    Environment = item.Attribute("Environment").Value,
                    IsDefault = item.Attribute("IsDefault").Value == "1",
                    IsActive = item.Attribute("IsActive").Value == "1",
                };
                listSiteUrl.Add(objSiteUrl);
            }

            ViewBag.LogData = listLogData;
            ViewBag.SiteUrl = listSiteUrl;
        }

        return PartialView("~/Views/Trace/_NAP_Site_InsertWithSiteURL.cshtml");
    }

    public PartialViewResult NAPMenuItemUpdate(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_MenuItem_Update)
        {
            var listLogData = new List<LogData>();
            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Menu_Item_Name,
                OldValue = elementOld.Attribute("MenuItemName").Value,
                NewValue = elementNew.Attribute("MenuItemName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Function_Name,
                OldValue = elementOld.Attribute("FunctionName").Value,
                NewValue = elementNew.Attribute("FunctionName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Site_Name,
                OldValue = elementOld.Attribute("SiteName").Value,
                NewValue = elementNew.Attribute("SiteName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Description,
                OldValue = elementOld.Attribute("Description").Value,
                NewValue = elementNew.Attribute("Description").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Parent,
                OldValue = elementOld.Attribute("ParentName").Value,
                NewValue = elementNew.Attribute("ParentName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Policy,
                OldValue = elementOld.Attribute("Policy").Value,
                NewValue = elementNew.Attribute("Policy").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.URL,
                OldValue = elementOld.Attribute("URL").Value,
                NewValue = elementNew.Attribute("URL").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Display_Order,
                OldValue = elementOld.Attribute("DisplayOrder").Value,
                NewValue = elementNew.Attribute("DisplayOrder").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Active,
                OldValue = elementOld.Attribute("IsActive").Value == "1" ? "true" : "false",
                NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
            });

            ViewBag.LogData = listLogData;
        }

        return PartialView("~/Views/Trace/_NAPLogUpdate.cshtml");
    }

    public PartialViewResult NAPMenuItemUpdateStatus(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_MenuItem_UpdateStatus)
        {
            var listLogData = new List<LogData>();

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Status,
                OldValue = elementOld.Attribute("IsActive").Value == "1" ? "Active" : "Unactive",
                NewValue = elementNew.Attribute("IsActive").Value == "1" ? "Active" : "Unactive"
            });

            ViewBag.LogData = listLogData;
        }

        return PartialView("~/Views/Trace/_NAPLogUpdate.cshtml");
    }

    public PartialViewResult NAPFunctionUpdate(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Function_Update
            || log.ActionId == (int)LogAction.NAP_Function_Duplicate)
        {
            var listLogData = new List<LogData>();
            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Function_Name,
                OldValue = elementOld.Attribute("FunctionName").Value,
                NewValue = elementNew.Attribute("FunctionName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Site_Name,
                OldValue = elementOld.Attribute("SiteName").Value,
                NewValue = elementNew.Attribute("SiteName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Function_Key,
                OldValue = elementOld.Attribute("FunctionKey").Value,
                NewValue = elementNew.Attribute("FunctionKey").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Description,
                OldValue = elementOld.Attribute("Description").Value,
                NewValue = elementNew.Attribute("Description").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Parent,
                OldValue = elementOld.Attribute("ParentName").Value,
                NewValue = elementNew.Attribute("ParentName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.CRUD,
                OldValue = elementOld.Attribute("CRUD").Value,
                NewValue = elementNew.Attribute("CRUD").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Display_Order,
                OldValue = elementOld.Attribute("DisplayOrder").Value,
                NewValue = elementNew.Attribute("DisplayOrder").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Active,
                OldValue = elementOld.Attribute("IsActive").Value == "1" ? "true" : "false",
                NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Environment,
                OldValue = elementOld.Attribute("Environment").Value,
                NewValue = elementNew.Attribute("Environment").Value
            });

            ViewBag.LogData = listLogData;
        }

        return PartialView("~/Views/Trace/_NAPLogUpdate.cshtml");
    }

    public PartialViewResult NAPFunctionUpdateStatus(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Function_UpdateStatus)
        {
            var listLogData = new List<LogData>();

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Status,
                OldValue = elementOld.Attribute("IsActive").Value == "1" ? "Active" : "Unactive",
                NewValue = elementNew.Attribute("IsActive").Value == "1" ? "Active" : "Unactive"
            });

            ViewBag.LogData = listLogData;
        }

        return PartialView("~/Views/Trace/_NAPLogUpdate.cshtml");
    }

    public PartialViewResult NAPSiteUpdate(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Site_Update)
        {
            var listLogData = new List<LogData>();
            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Site_Name,
                OldValue = elementOld.Attribute("SiteName").Value,
                NewValue = elementNew.Attribute("SiteName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Site_Key,
                OldValue = elementOld.Attribute("SiteKey").Value,
                NewValue = elementNew.Attribute("SiteKey").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Secret_Access_Key,
                OldValue = elementOld.Attribute("SecretAccessKey").Value,
                NewValue = elementNew.Attribute("SecretAccessKey").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Description,
                OldValue = elementOld.Attribute("Description").Value,
                NewValue = elementNew.Attribute("Description").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Environment,
                OldValue = elementOld.Attribute("Environment").Value,
                NewValue = elementNew.Attribute("Environment").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Active,
                OldValue = elementOld.Attribute("IsActive").Value == "1" ? "true" : "false",
                NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
            });

            ViewBag.LogData = listLogData;
        }

        return PartialView("~/Views/Trace/_NAPLogUpdate.cshtml");
    }

    public PartialViewResult NAPSiteUpdateStatus(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Site_UpdateStatus)
        {
            var listLogData = new List<LogData>();

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Status,
                OldValue = elementOld.Attribute("IsActive").Value == "1" ? "Active" : "Unactive",
                NewValue = elementNew.Attribute("IsActive").Value == "1" ? "Active" : "Unactive"
            });

            ViewBag.LogData = listLogData;
        }

        return PartialView("~/Views/Trace/_NAPLogUpdate.cshtml");
    }

    public PartialViewResult NAPGroupUpdate(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Group_Update)
        {
            var listLogData = new List<LogData>();
            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Group_Name,
                OldValue = elementOld.Attribute("GroupName").Value,
                NewValue = elementNew.Attribute("GroupName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Description,
                OldValue = elementOld.Attribute("Description").Value,
                NewValue = elementNew.Attribute("Description").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Active,
                OldValue = elementOld.Attribute("IsActive").Value == "1" ? "true" : "false",
                NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
            });

            ViewBag.LogData = listLogData;
        }

        return PartialView("~/Views/Trace/_NAPLogUpdate.cshtml");
    }

    public PartialViewResult NAPGroupUpdateStatus(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Group_UpdateStatus)
        {
            var listLogData = new List<LogData>();

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Status,
                OldValue = elementOld.Attribute("IsActive").Value == "1" ? "Active" : "Unactive",
                NewValue = elementNew.Attribute("IsActive").Value == "1" ? "Active" : "Unactive"
            });

            ViewBag.LogData = listLogData;
        }

        return PartialView("~/Views/Trace/_NAPLogUpdate.cshtml");
    }

    public PartialViewResult NAPCustUpdate(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data ?? "<ROOT/>");
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Cust_Update)
        {
            var listLogData = new List<LogData>();

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Group_Name,
                OldValue = elementOld.Attribute("GroupName").Value,
                NewValue = elementNew.Attribute("GroupName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.First_Name,
                OldValue = elementOld.Attribute("FirstName").Value,
                NewValue = elementNew.Attribute("FirstName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Last_Name,
                OldValue = elementOld.Attribute("LastName").Value,
                NewValue = elementNew.Attribute("LastName").Value
            });

            listLogData.Add(new LogData
            {
                FieldName = ViewResources.Email,
                OldValue = elementOld.Attribute("Email").Value,
                NewValue = elementNew.Attribute("Email").Value
            });

            ViewBag.LogData = listLogData;
        }

        return PartialView("~/Views/Trace/_NAPLogUpdate.cshtml");
    }

    public PartialViewResult NAPSiteUrl(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Site_UpdateSiteURL
            || log.ActionId == (int)LogAction.NAP_Site_InsertSiteURL
            || log.ActionId == (int)LogAction.NAP_Site_UpdateDefaultSiteURL)
        {
            var strUrlOld = elementOld.Attribute("Value").Value.DecodeXMLValue();

            var listSiteUrlOld = new List<SiteUrl>();
            var documentSiteUrlOld = XDocument.Parse(strUrlOld);
            var siteUrlOld = documentSiteUrlOld.Descendants("ITEM");

            foreach (var item in siteUrlOld)
            {
                var objSiteUrl = new SiteUrl
                {
                    Url = item.Attribute("URL").Value,
                    Environment = item.Attribute("Environment").Value,
                    IsDefault = item.Attribute("IsDefault").Value == "1",
                    IsActive = item.Attribute("IsActive").Value == "1",
                };
                listSiteUrlOld.Add(objSiteUrl);
            }

            ViewBag.SiteUrlOld = listSiteUrlOld;

            var strUrl = elementNew.Attribute("Value").Value.DecodeXMLValue();

            var listSiteUrl = new List<SiteUrl>();
            var documentSiteUrl = XDocument.Parse(strUrl);
            var siteUrl = documentSiteUrl.Descendants("ITEM");

            foreach (var item in siteUrl)
            {
                var objSiteUrl = new SiteUrl
                {
                    Url = item.Attribute("URL").Value,
                    Environment = item.Attribute("Environment").Value,
                    IsDefault = item.Attribute("IsDefault").Value == "1",
                    IsActive = item.Attribute("IsActive").Value == "1",
                };
                listSiteUrl.Add(objSiteUrl);
            }

            ViewBag.SiteUrl = listSiteUrl;
        }

        return PartialView("~/Views/Trace/_NAP_Site_Url.cshtml");
    }

    public PartialViewResult NAPAssignPermission(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Group_AssignFunction
            || log.ActionId == (int)LogAction.NAP_User_AssignFunction
            || log.ActionId == (int)LogAction.NAP_Function_CopyPermission)
        {
            var siteId = Convert.ToInt32(elementNew.Attribute("SiteId").Value);

            ViewBag.FunctionList = _functionService.GetBySiteInHierarchy(siteId, Environment, true);


            if (elementOld.Attribute("Value") != null)
            {
                var strFunctionPolicy = elementOld.Attribute("Value").Value.DecodeXMLValue();

                if (!string.IsNullOrWhiteSpace(strFunctionPolicy))
                {
                    ViewBag.ListOldData = GetOldFunctionPolicyList(strFunctionPolicy);
                }
            }

            if (elementNew.Attribute("Value") != null)
            {
                var strFunctionPolicy = elementNew.Attribute("Value").Value.DecodeXMLValue();

                if (!string.IsNullOrWhiteSpace(strFunctionPolicy))
                {
                    ViewBag.ListNewData = GetNewFunctionPolicyList(strFunctionPolicy);
                }
            }

            Site site = _siteService.GetById(siteId);

            ViewBag.SiteName = site.Name;
        }

        #endregion View log

        return PartialView("~/Views/Trace/_NAP_Assign_Permission.cshtml");
    }

    private static Hashtable GetNewFunctionPolicyList(string strFunctionPolicy)
    {
        var hashNewPolicy = new Hashtable();

        var documentFunctionPolicy = XDocument.Parse(strFunctionPolicy);
        var functionPolicy = documentFunctionPolicy.Descendants("ITEM");

        foreach (var item in functionPolicy)
        {
            var id = Convert.ToInt16(item.Attribute("FunctionId").Value);
            var policy = Convert.ToInt32(item.Attribute("Policy").Value);

            if (!hashNewPolicy.Contains(id))
            {
                hashNewPolicy.Add(id, policy);
            }
        }

        return hashNewPolicy;
    }

    private List<FunctionPolicy> GetOldFunctionPolicyList(string strFunctionPolicy)
    {
        var listFunctionPolicyOld = new List<FunctionPolicy>();
        var documentFunctionPolicy = XDocument.Parse(strFunctionPolicy);
        var functionPolicy = documentFunctionPolicy.Descendants("ITEM");

        foreach (var item in functionPolicy)
        {
            var objFunctionPolicy = new FunctionPolicy
            {
                Function = new Function
                {
                    Id = Convert.ToInt16(item.Attribute("FunctionId").Value)
                },
                Policy = Convert.ToInt32(item.Attribute("Policy").Value)
            };

            listFunctionPolicyOld.Add(objFunctionPolicy);
        }

        return listFunctionPolicyOld;
    }

    public PartialViewResult NAPAssignSite(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_User_AssignSite)
        {
            if (elementOld.Attribute("Value") != null
                && !string.IsNullOrWhiteSpace(elementOld.Attribute("Value").Value))
            {
                var strUrlOld = elementOld.Attribute("Value").Value.DecodeXMLValue();

                var listSiteUrlOld = new List<SiteLevel>();
                var documentSiteUrlOld = XDocument.Parse(strUrlOld);
                var siteUrlOld = documentSiteUrlOld.Descendants("ITEM");

                foreach (var item in siteUrlOld)
                {
                    var objSiteUrl = new SiteLevel
                    {
                        Site = new Site
                        {
                            Id = Convert.ToByte(item.Attribute("SiteId").Value),
                            Name = item.Attribute("SiteName").Value
                        },
                        Level = Convert.ToInt32(item.Attribute("Level").Value),
                        IsActive = item.Attribute("IsActive").Value == "1"
                    };
                    listSiteUrlOld.Add(objSiteUrl);
                }

                ViewBag.SiteUrlOld = listSiteUrlOld;
            }

            if (elementNew.Attribute("Value") != null)
            {
                var strUrl = elementNew.Attribute("Value").Value.DecodeXMLValue();

                var listSiteUrl = new List<SiteLevel>();
                var documentSiteUrl = XDocument.Parse(strUrl);
                var siteUrl = documentSiteUrl.Descendants("ITEM");
                foreach (var item in siteUrl)
                {
                    var objSiteUrl = new SiteLevel
                    {
                        Site = new Site
                        {
                            Id = Convert.ToByte(item.Attribute("SiteId").Value),
                            Name = item.Attribute("SiteName").Value
                        },
                        Level = Convert.ToInt32(item.Attribute("Level").Value),
                        IsActive = item.Attribute("IsActive").Value == "1"
                    };
                    listSiteUrl.Add(objSiteUrl);
                }

                ViewBag.SiteUrl = listSiteUrl;
            }
        }

        return PartialView("~/Views/Trace/_NAP_Assign_Site.cshtml");
    }

    public PartialViewResult NAPAssignNewSite(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);

        if (log.ActionId == (int)LogAction.NAP_User_AssignNewSite)
        {
            var newSite = new SiteLevel
            {
                Site = new Site
                {
                    Id = Convert.ToByte(elementNew.Attribute("SiteId").Value),
                    Name = Convert.ToString(elementNew.Attribute("SiteName").Value)
                },
                Level = Convert.ToInt32(elementNew.Attribute("Level").Value),
                IsActive = elementNew.Attribute("IsActive").Value == "1"
            };

            ViewBag.NewLog = newSite;
        }

        return PartialView("~/Views/Trace/_NAP_Assign_New_Site.cshtml");
    }

    public PartialViewResult NAPUpdateSite(LogExecSp log)
    {
        var newElement = XElement.Parse(log.Data);
        var oldElement = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (oldElement.Attribute("UserId") != null)
        {
            var oldSite = new SiteLevel
            {
                Site = new Site
                {
                    Id = Convert.ToByte(oldElement.Attribute("SiteId").Value),
                    Name = Convert.ToString(oldElement.Attribute("SiteName").Value)
                },
                Level = Convert.ToInt32(oldElement.Attribute("Level").Value),
                IsActive = oldElement.Attribute("IsActive").Value == "1",
                IsNeverExpired = oldElement.Attribute("IsNeverExpired").Value == "1"
            };

            DateTime expiredDate;
            DateTime.TryParse(oldElement.Attribute("ExpiredDate").Value, out expiredDate);
            oldSite.ExpiredDate = expiredDate;

            ViewBag.oldSite = oldSite;
        }

        if (newElement.Attribute("UserId") != null)
        {
            var newSite = new SiteLevel
            {
                Site = new Site
                {
                    Id = Convert.ToByte(newElement.Attribute("SiteId").Value),
                    Name = Convert.ToString(newElement.Attribute("SiteName").Value)
                },
                Level = Convert.ToInt32(newElement.Attribute("Level").Value),
                IsActive = newElement.Attribute("IsActive").Value == "1",
                IsNeverExpired = newElement.Attribute("IsNeverExpired").Value == "1"
            };

            DateTime expiredDate;
            DateTime.TryParse(newElement.Attribute("ExpiredDate").Value, out expiredDate);
            newSite.ExpiredDate = expiredDate;

            ViewBag.newSite = newSite;
        }

        return PartialView("~/Views/Trace/_NAP_Update_Site.cshtml");
    }

    public PartialViewResult NAPFunctionAssignToUsers(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Function_AssignFunctionToUsers)
        {
            var strUserPolicyOld = elementOld.Attribute("Value").Value.DecodeXMLValue();

            var listUserPolicyOld = new List<UserPolicy>();
            var documentUserPolicyOld = XDocument.Parse(strUserPolicyOld);
            var userPolicyOld = documentUserPolicyOld.Descendants("ITEM");

            foreach (var item in userPolicyOld)
            {
                var userPolicy = new UserPolicy
                {
                    User = new User
                    {
                        Username = item.Attribute("UserName").Value
                    },
                    Policy = Convert.ToInt32(item.Attribute("Policy").Value)
                };
                listUserPolicyOld.Add(userPolicy);
            }

            ViewBag.UserPolicyOld = listUserPolicyOld;

            var strUrl = elementNew.Attribute("Value").Value.DecodeXMLValue();

            var listUserPolicy = new List<UserPolicy>();
            var documentUserPolicy = XDocument.Parse(strUrl);
            var userPolicyNew = documentUserPolicy.Descendants("ITEM");

            foreach (var item in userPolicyNew)
            {
                var userPolicy = new UserPolicy
                {
                    User = new User
                    {
                        Username = item.Attribute("UserName").Value
                    },
                    Policy = Convert.ToInt32(item.Attribute("Policy").Value)
                };
                listUserPolicy.Add(userPolicy);
            }

            ViewBag.UserPolicyNew = listUserPolicy;
        }

        return PartialView("~/Views/Trace/_NAP_Function_AssignToUsers.cshtml");
    }

    public PartialViewResult NAPFunctionAssignFunctionToGroups(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Function_AssignFunctionToGroups)
        {
            var strGroupPolicyOld = elementOld.Attribute("Value").Value.DecodeXMLValue();

            var listGroupPolicyOld = new List<GroupPolicy>();
            var documentGroupPolicyOld = XDocument.Parse(strGroupPolicyOld);
            var groupPolicyOld = documentGroupPolicyOld.Descendants("ITEM");

            foreach (var item in groupPolicyOld)
            {
                var groupPolicy = new GroupPolicy
                {
                    Group = new Group
                    {
                        Name = item.Attribute("GroupName").Value
                    },
                    Policy = Convert.ToInt32(item.Attribute("Policy").Value)
                };
                listGroupPolicyOld.Add(groupPolicy);
            }

            ViewBag.GroupPolicyOld = listGroupPolicyOld;

            var strUrl = elementNew.Attribute("Value").Value.DecodeXMLValue();

            var listGroupPolicy = new List<GroupPolicy>();
            var documentGroupPolicy = XDocument.Parse(strUrl);
            var groupPolicyNew = documentGroupPolicy.Descendants("ITEM");

            foreach (var item in groupPolicyNew)
            {
                var groupPolicy = new GroupPolicy
                {
                    Group = new Group
                    {
                        Name = item.Attribute("GroupName").Value
                    },
                    Policy = Convert.ToInt32(item.Attribute("Policy").Value)
                };
                listGroupPolicy.Add(groupPolicy);
            }

            ViewBag.GroupPolicyNew = listGroupPolicy;
        }

        return PartialView("~/Views/Trace/_NAP_Function_AssignToGroups.cshtml");
    }

    public PartialViewResult TaskNAPRevokeUserPermission(LogExecSp log)
    {
        var taskNAPRevokeUserPermissionVM = new TaskNAPRevokeUserPermissionVM();

        var element = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.Task_NAP_RevokeUserPermission && element != null)
        {
            var elementValue = element.Attribute("Value").Value.DecodeXMLValue();

            if (!string.IsNullOrWhiteSpace(elementValue))
            {
                var items = XDocument.Parse(elementValue).Descendants("ITEM");

                foreach (var item in items)
                {
                    var revokedSite = new RevokedSite
                    {
                        SiteId = Convert.ToInt32(item.Attribute("SiteId").Value),
                        SiteName = item.Attribute("SiteName").Value
                    };

                    taskNAPRevokeUserPermissionVM.RevokedSites.Add(revokedSite);
                }
            }
        }

        return PartialView("~/Views/Trace/_Task_NAP_RevokeUserPermission.cshtml", taskNAPRevokeUserPermissionVM);
    }
}
