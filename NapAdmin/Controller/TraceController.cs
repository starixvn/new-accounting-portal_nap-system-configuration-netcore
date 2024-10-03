namespace NapAdmin.Controller;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fanex.NAP.Client.Attributes;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client;
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

public class TraceController(ITraceService traceService,
        IFunctionService functionService,
        ISiteService siteService,
        INapSession napSession,
        INapSettings napSettings,
        IUserService userService)
    : BaseController(napSession, napSettings, userService)
{
    #region Log Exec Sp

    [NapAuth(FunctionKey = FunctionKeys.ExecSpLog)]
    [HttpGet]
    public ActionResult LogExecSp(
        int page = 1,
        int size = 20,
        string q = EmptyString,
        string username = EmptyString,
        DateTime? from = null,
        DateTime? to = null,
        int filterBy = 1)
    {
        var criteria = new GetAllLogCriteria
        {
            PageIndex = page,
            PageSize = size,
            Name = q ?? string.Empty,
            Username = username,
            From = from ?? DateTime.Now,
            To = to ?? DateTime.Now.AddDays(1),
            FilterType = filterBy,
            TotalRows = 0
        };

        var listLog = traceService.SearchExecSp(criteria);
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
        int page = 1,
        int size = 20,
        string q = EmptyString,
        DateTime? from = null,
        DateTime? to = null,
        int siteId = 0)
    {
        var criteria = new GetAllLogUserLoginCriteria
        {
            PageIndex = page,
            PageSize = size,
            Name = q ?? string.Empty,
            From = from ?? DateTime.Now,
            To = to ?? DateTime.Now.AddDays(1),
            SiteId = siteId,
            TotalRows = 0
        };

        var listLog = traceService.SearchUserLogin(criteria);

        var listSite = siteService.GetAll().ToList();
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
        var log = traceService.GetById(id);
        return log.ActionId switch
        {
            (int)LogAction.NAP_Cust_Insert => NAPCustInsert(log),
            (int)LogAction.NAP_User_Insert => NAPUserInsert(log),
            (int)LogAction.NAP_Function_Insert => NAPFunctionInsert(log),
            (int)LogAction.NAP_Group_Insert => NAPGroupInsert(log),
            (int)LogAction.NAP_MenuItem_Insert => NAPMenuItemInsert(log),
            (int)LogAction.NAP_Site_InsertWithSiteURL => NAPSiteInsertWithSiteUrl(log),
            (int)LogAction.NAP_MenuItem_Update => NAPMenuItemUpdate(log),
            (int)LogAction.NAP_MenuItem_UpdateStatus => NAPMenuItemUpdateStatus(log),
            (int)LogAction.NAP_Function_Update or (int)LogAction.NAP_Function_Duplicate => NAPFunctionUpdate(log),
            (int)LogAction.NAP_Function_UpdateStatus => NAPFunctionUpdateStatus(log),
            (int)LogAction.NAP_Site_Update => NAPSiteUpdate(log),
            (int)LogAction.NAP_Site_UpdateStatus => NAPSiteUpdateStatus(log),
            (int)LogAction.NAP_Group_Update => NAPGroupUpdate(log),
            (int)LogAction.NAP_Group_UpdateStatus => NAPGroupUpdateStatus(log),
            (int)LogAction.NAP_Cust_Update => NAPCustUpdate(log),
            (int)LogAction.NAP_Site_UpdateSiteURL or (int)LogAction.NAP_Site_InsertSiteURL
                or (int)LogAction.NAP_Site_UpdateDefaultSiteURL => NAPSiteUrl(log),
            (int)LogAction.NAP_Group_AssignFunction or (int)LogAction.NAP_User_AssignFunction
                or (int)LogAction.NAP_Function_CopyPermission => NAPAssignPermission(log),
            (int)LogAction.NAP_User_AssignNewSite => NAPAssignNewSite(log),
            (int)LogAction.NAP_User_UpdateSite => NAPUpdateSite(log),
            (int)LogAction.NAP_User_AssignSite => NAPAssignSite(log),
            (int)LogAction.NAP_Function_AssignFunctionToUsers => NAPFunctionAssignToUsers(log),
            (int)LogAction.NAP_Function_AssignFunctionToGroups => NAPFunctionAssignFunctionToGroups(log),
            (int)LogAction.Task_NAP_RevokeUserPermission => TaskNAPRevokeUserPermission(log),
            _ => View("~/Views/Trace/_NAP_Empty_LogDetail.cshtml")
        };
    }
    

    #endregion Ajax

    #region View log

    public ActionResult NAPUserInsert(LogExecSp log)
    {
        if (log.ActionId == (int)LogAction.NAP_User_Insert)
        {
            var elementNew = XElement.Parse(log.Data);

            var listLogData = new List<LogData>
            {
                new()
                {
                    FieldName = ViewResources.Username,
                    NewValue = elementNew.Attribute("Username").Value
                },
                new()
                {
                    FieldName = ViewResources.Group_Name,
                    NewValue = elementNew.Attribute("GroupName").Value
                }
            };

            ViewBag.LogData = listLogData;
        }

        return View("~/Views/Trace/_NAPLogInsert.cshtml");
    }

    public ActionResult NAPCustInsert(LogExecSp log)
    {
        var element = XElement.Parse(log.Data);

        if (log.ActionId == (int)LogAction.NAP_Cust_Insert)
        {
            var listLogData = new List<LogData>
            {
                new()
                {
                    FieldName = ViewResources.Username,
                    NewValue = element.Attribute("Username").Value
                },
                new()
                {
                    FieldName = ViewResources.Group_Name,
                    NewValue = element.Attribute("GroupName").Value
                },
                new()
                {
                    FieldName = ViewResources.First_Name,
                    NewValue = element.Attribute("FirstName").Value
                },
                new()
                {
                    FieldName = ViewResources.Last_Name,
                    NewValue = element.Attribute("LastName").Value
                },
                new()
                {
                    FieldName = ViewResources.Email,
                    NewValue = element.Attribute("Email").Value
                }
            };

            ViewBag.LogData = listLogData;
        }

        return View("~/Views/Trace/_NAPLogInsert.cshtml");
    }

    public ActionResult NAPFunctionInsert(LogExecSp log)
    {
        var element = XElement.Parse(log.Data);

        if (log.ActionId == (int)LogAction.NAP_Function_Insert)
        {
            var listLogData = new List<LogData>
            {
                new()
                {
                    FieldName = ViewResources.Function_Name,
                    NewValue = element.Attribute("FunctionName").Value
                },
                new()
                {
                    FieldName = ViewResources.Site_Name,
                    NewValue = element.Attribute("SiteName").Value
                },
                new()
                {
                    FieldName = ViewResources.Function_Key,
                    NewValue = element.Attribute("FunctionKey").Value
                },
                new()
                {
                    FieldName = ViewResources.Description,
                    NewValue = element.Attribute("Description").Value
                },
                new()
                {
                    FieldName = ViewResources.Parent,
                    NewValue = element.Attribute("ParentName").Value
                },
                new()
                {
                    FieldName = ViewResources.CRUD,
                    NewValue = element.Attribute("CRUD").Value
                },
                new()
                {
                    FieldName = ViewResources.Display_Order,
                    NewValue = element.Attribute("DisplayOrder").Value
                },
                new()
                {
                    FieldName = ViewResources.Active,
                    NewValue = element.Attribute("IsActive").Value == "1" ? "true" : "false"
                },
                new()
                {
                    FieldName = ViewResources.Environment,
                    NewValue = element.Attribute("Environment").Value
                }
            };

            ViewBag.LogData = listLogData;
        }

        return View("~/Views/Trace/_NAPLogInsert.cshtml");
    }

    public ActionResult NAPGroupInsert(LogExecSp log)
    {
        var element = XElement.Parse(log.Data);

        if (log.ActionId == (int)LogAction.NAP_Group_Insert)
        {
            var listLogData = new List<LogData>
            {
                new()
                {
                    FieldName = ViewResources.Group_Name,
                    NewValue = element.Attribute("GroupName").Value
                },
                new()
                {
                    FieldName = ViewResources.Description,
                    NewValue = element.Attribute("Description").Value
                },
                new()
                {
                    FieldName = ViewResources.Active,
                    NewValue = element.Attribute("IsActive").Value == "1" ? "true" : "false"
                }
            };

            ViewBag.LogData = listLogData;
        }

        return View("~/Views/Trace/_NAPLogInsert.cshtml");
    }

    public ActionResult NAPMenuItemInsert(LogExecSp log)
    {
        var element = XElement.Parse(log.Data);

        if (log.ActionId == (int)LogAction.NAP_MenuItem_Insert)
        {
            var listLogData = new List<LogData>
            {
                new()
                {
                    FieldName = ViewResources.Menu_Item_Name,
                    NewValue = element.Attribute("MenuItemName").Value
                },
                new()
                {
                    FieldName = ViewResources.Function_Name,
                    NewValue = element.Attribute("FunctionName").Value
                },
                new()
                {
                    FieldName = ViewResources.Site_Name,
                    NewValue = element.Attribute("SiteName").Value
                },
                new()
                {
                    FieldName = ViewResources.Description,
                    NewValue = element.Attribute("Description").Value
                },
                new()
                {
                    FieldName = ViewResources.Parent,
                    NewValue = element.Attribute("ParentName").Value
                },
                new()
                {
                    FieldName = ViewResources.Policy,
                    NewValue = element.Attribute("Policy").Value
                },
                new()
                {
                    FieldName = ViewResources.URL,
                    NewValue = element.Attribute("URL").Value
                },
                new()
                {
                    FieldName = ViewResources.Display_Order,
                    NewValue = element.Attribute("DisplayOrder").Value
                },
                new()
                {
                    FieldName = ViewResources.Active,
                    NewValue = element.Attribute("IsActive").Value == "1" ? "true" : "false"
                }
            };

            ViewBag.LogData = listLogData;
        }

        return View("~/Views/Trace/_NAPLogInsert.cshtml");
    }

    public ActionResult NAPSiteInsertWithSiteUrl(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);

        if (log.ActionId == (int)LogAction.NAP_Site_InsertWithSiteURL)
        {
            var listLogData = new List<LogData>
            {
                new()
                {
                    FieldName = ViewResources.Site_Name,
                    NewValue = elementNew.Attribute("SiteName").Value
                },
                new()
                {
                    FieldName = ViewResources.Site_Key,
                    NewValue = elementNew.Attribute("SiteKey").Value
                },
                new()
                {
                    FieldName = ViewResources.Secret_Access_Key,
                    NewValue = elementNew.Attribute("SecretAccessKey").Value
                },
                new()
                {
                    FieldName = ViewResources.Description,
                    NewValue = elementNew.Attribute("Description").Value
                },
                new()
                {
                    FieldName = ViewResources.Environment,
                    NewValue = elementNew.Attribute("Environment").Value
                },
                new()
                {
                    FieldName = ViewResources.Active,
                    NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
                }
            };

            var strUrl = elementNew.Attribute("Value").Value.DecodeXMLValue();

            var documentSiteUrl = XDocument.Parse(strUrl);
            var siteUrl = documentSiteUrl.Descendants("ITEM");

            var listSiteUrl = siteUrl.Select(item => new SiteUrl
                {
                    Url = item.Attribute("URL").Value, 
                    Environment = item.Attribute("Environment").Value, 
                    IsDefault = item.Attribute("IsDefault").Value == "1",
                    IsActive = item.Attribute("IsActive").Value == "1"
                })
                .ToList();

            ViewBag.LogData = listLogData;
            ViewBag.SiteUrl = listSiteUrl;
        }

        return View("~/Views/Trace/_NAP_Site_InsertWithSiteURL.cshtml");
    }

    public ActionResult NAPMenuItemUpdate(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_MenuItem_Update)
        {
            var listLogData = new List<LogData>
            {
                new()
                {
                    FieldName = ViewResources.Menu_Item_Name,
                    OldValue = elementOld.Attribute("MenuItemName").Value,
                    NewValue = elementNew.Attribute("MenuItemName").Value
                },
                new()
                {
                    FieldName = ViewResources.Function_Name,
                    OldValue = elementOld.Attribute("FunctionName").Value,
                    NewValue = elementNew.Attribute("FunctionName").Value
                },
                new()
                {
                    FieldName = ViewResources.Site_Name,
                    OldValue = elementOld.Attribute("SiteName").Value,
                    NewValue = elementNew.Attribute("SiteName").Value
                },
                new()
                {
                    FieldName = ViewResources.Description,
                    OldValue = elementOld.Attribute("Description").Value,
                    NewValue = elementNew.Attribute("Description").Value
                },
                new()
                {
                    FieldName = ViewResources.Parent,
                    OldValue = elementOld.Attribute("ParentName").Value,
                    NewValue = elementNew.Attribute("ParentName").Value
                },
                new()
                {
                    FieldName = ViewResources.Policy,
                    OldValue = elementOld.Attribute("Policy").Value,
                    NewValue = elementNew.Attribute("Policy").Value
                },
                new()
                {
                    FieldName = ViewResources.URL,
                    OldValue = elementOld.Attribute("URL").Value,
                    NewValue = elementNew.Attribute("URL").Value
                },
                new()
                {
                    FieldName = ViewResources.Display_Order,
                    OldValue = elementOld.Attribute("DisplayOrder").Value,
                    NewValue = elementNew.Attribute("DisplayOrder").Value
                },
                new()
                {
                    FieldName = ViewResources.Active,
                    OldValue = elementOld.Attribute("IsActive").Value == "1" ? "true" : "false",
                    NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
                }
            };

            ViewBag.LogData = listLogData;
        }

        return View("~/Views/Trace/_NAPLogUpdate.cshtml");
    }

    public ActionResult NAPMenuItemUpdateStatus(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_MenuItem_UpdateStatus)
        {
            var listLogData = new List<LogData>
            {
                new()
                {
                    FieldName = ViewResources.Status,
                    OldValue = elementOld.Attribute("IsActive").Value == "1" ? "Active" : "Unactive",
                    NewValue = elementNew.Attribute("IsActive").Value == "1" ? "Active" : "Unactive"
                }
            };

            ViewBag.LogData = listLogData;
        }

        return View("~/Views/Trace/_NAPLogUpdate.cshtml");
    }

    public ActionResult NAPFunctionUpdate(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId is (int)LogAction.NAP_Function_Update or (int)LogAction.NAP_Function_Duplicate)
        {
            var listLogData = new List<LogData>
            {
                new()
                {
                    FieldName = ViewResources.Function_Name,
                    OldValue = elementOld.Attribute("FunctionName").Value,
                    NewValue = elementNew.Attribute("FunctionName").Value
                },
                new()
                {
                    FieldName = ViewResources.Site_Name,
                    OldValue = elementOld.Attribute("SiteName").Value,
                    NewValue = elementNew.Attribute("SiteName").Value
                },
                new()
                {
                    FieldName = ViewResources.Function_Key,
                    OldValue = elementOld.Attribute("FunctionKey").Value,
                    NewValue = elementNew.Attribute("FunctionKey").Value
                },
                new()
                {
                    FieldName = ViewResources.Description,
                    OldValue = elementOld.Attribute("Description").Value,
                    NewValue = elementNew.Attribute("Description").Value
                },
                new()
                {
                    FieldName = ViewResources.Parent,
                    OldValue = elementOld.Attribute("ParentName").Value,
                    NewValue = elementNew.Attribute("ParentName").Value
                },
                new()
                {
                    FieldName = ViewResources.CRUD,
                    OldValue = elementOld.Attribute("CRUD").Value,
                    NewValue = elementNew.Attribute("CRUD").Value
                },
                new()
                {
                    FieldName = ViewResources.Display_Order,
                    OldValue = elementOld.Attribute("DisplayOrder").Value,
                    NewValue = elementNew.Attribute("DisplayOrder").Value
                },
                new()
                {
                    FieldName = ViewResources.Active,
                    OldValue = elementOld.Attribute("IsActive").Value == "1" ? "true" : "false",
                    NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
                },
                new()
                {
                    FieldName = ViewResources.Environment,
                    OldValue = elementOld.Attribute("Environment").Value,
                    NewValue = elementNew.Attribute("Environment").Value
                }
            };

            ViewBag.LogData = listLogData;
        }

        return View("~/Views/Trace/_NAPLogUpdate.cshtml");
    }

    public ActionResult NAPFunctionUpdateStatus(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Function_UpdateStatus)
        {
            var listLogData = new List<LogData>
            {
                new()
                {
                    FieldName = ViewResources.Status,
                    OldValue = elementOld.Attribute("IsActive").Value == "1" ? "Active" : "Unactive",
                    NewValue = elementNew.Attribute("IsActive").Value == "1" ? "Active" : "Unactive"
                }
            };

            ViewBag.LogData = listLogData;
        }

        return View("~/Views/Trace/_NAPLogUpdate.cshtml");
    }

    public ActionResult NAPSiteUpdate(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Site_Update)
        {
            var listLogData = new List<LogData>
            {
                new()
                {
                    FieldName = ViewResources.Site_Name,
                    OldValue = elementOld.Attribute("SiteName").Value,
                    NewValue = elementNew.Attribute("SiteName").Value
                },
                new()
                {
                    FieldName = ViewResources.Site_Key,
                    OldValue = elementOld.Attribute("SiteKey").Value,
                    NewValue = elementNew.Attribute("SiteKey").Value
                },
                new()
                {
                    FieldName = ViewResources.Secret_Access_Key,
                    OldValue = elementOld.Attribute("SecretAccessKey").Value,
                    NewValue = elementNew.Attribute("SecretAccessKey").Value
                },
                new()
                {
                    FieldName = ViewResources.Description,
                    OldValue = elementOld.Attribute("Description").Value,
                    NewValue = elementNew.Attribute("Description").Value
                },
                new()
                {
                    FieldName = ViewResources.Environment,
                    OldValue = elementOld.Attribute("Environment").Value,
                    NewValue = elementNew.Attribute("Environment").Value
                },
                new()
                {
                    FieldName = ViewResources.Active,
                    OldValue = elementOld.Attribute("IsActive").Value == "1" ? "true" : "false",
                    NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
                }
            };

            ViewBag.LogData = listLogData;
        }

        return View("~/Views/Trace/_NAPLogUpdate.cshtml");
    }

    public ActionResult NAPSiteUpdateStatus(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Site_UpdateStatus)
        {
            var listLogData = new List<LogData>
            {
                new()
                {
                    FieldName = ViewResources.Status,
                    OldValue = elementOld.Attribute("IsActive").Value == "1" ? "Active" : "Unactive",
                    NewValue = elementNew.Attribute("IsActive").Value == "1" ? "Active" : "Unactive"
                }
            };

            ViewBag.LogData = listLogData;
        }

        return View("~/Views/Trace/_NAPLogUpdate.cshtml");
    }

    public ActionResult NAPGroupUpdate(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Group_Update)
        {
            var listLogData = new List<LogData>
            {
                new()
                {
                    FieldName = ViewResources.Group_Name,
                    OldValue = elementOld.Attribute("GroupName").Value,
                    NewValue = elementNew.Attribute("GroupName").Value
                },
                new()
                {
                    FieldName = ViewResources.Description,
                    OldValue = elementOld.Attribute("Description").Value,
                    NewValue = elementNew.Attribute("Description").Value
                },
                new()
                {
                    FieldName = ViewResources.Active,
                    OldValue = elementOld.Attribute("IsActive").Value == "1" ? "true" : "false",
                    NewValue = elementNew.Attribute("IsActive").Value == "1" ? "true" : "false"
                }
            };

            ViewBag.LogData = listLogData;
        }

        return View("~/Views/Trace/_NAPLogUpdate.cshtml");
    }

    public ActionResult NAPGroupUpdateStatus(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Group_UpdateStatus)
        {
            var listLogData = new List<LogData>
            {
                new()
                {
                    FieldName = ViewResources.Status,
                    OldValue = elementOld.Attribute("IsActive").Value == "1" ? "Active" : "Unactive",
                    NewValue = elementNew.Attribute("IsActive").Value == "1" ? "Active" : "Unactive"
                }
            };

            ViewBag.LogData = listLogData;
        }

        return View("~/Views/Trace/_NAPLogUpdate.cshtml");
    }

    public ActionResult NAPCustUpdate(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data ?? "<ROOT/>");
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Cust_Update)
        {
            var listLogData = new List<LogData>
            {
                new()
                {
                    FieldName = ViewResources.Group_Name,
                    OldValue = elementOld.Attribute("GroupName").Value,
                    NewValue = elementNew.Attribute("GroupName").Value
                },
                new()
                {
                    FieldName = ViewResources.First_Name,
                    OldValue = elementOld.Attribute("FirstName").Value,
                    NewValue = elementNew.Attribute("FirstName").Value
                },
                new()
                {
                    FieldName = ViewResources.Last_Name,
                    OldValue = elementOld.Attribute("LastName").Value,
                    NewValue = elementNew.Attribute("LastName").Value
                },
                new()
                {
                    FieldName = ViewResources.Email,
                    OldValue = elementOld.Attribute("Email").Value,
                    NewValue = elementNew.Attribute("Email").Value
                }
            };

            ViewBag.LogData = listLogData;
        }

        return View("~/Views/Trace/_NAPLogUpdate.cshtml");
    }

    public ActionResult NAPSiteUrl(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId is (int)LogAction.NAP_Site_UpdateSiteURL or (int)LogAction.NAP_Site_InsertSiteURL or (int)LogAction.NAP_Site_UpdateDefaultSiteURL)
        {
            var strUrlOld = elementOld.Attribute("Value").Value.DecodeXMLValue();

            var documentSiteUrlOld = XDocument.Parse(strUrlOld);
            var siteUrlOld = documentSiteUrlOld.Descendants("ITEM");

            var listSiteUrlOld = siteUrlOld.Select(item => new SiteUrl
                {
                    Url = item.Attribute("URL").Value, 
                    Environment = item.Attribute("Environment").Value, 
                    IsDefault = item.Attribute("IsDefault").Value == "1", 
                    IsActive = item.Attribute("IsActive").Value == "1"
                })
                .ToList();

            ViewBag.SiteUrlOld = listSiteUrlOld;

            var strUrl = elementNew.Attribute("Value").Value.DecodeXMLValue();

            var documentSiteUrl = XDocument.Parse(strUrl);
            var siteUrl = documentSiteUrl.Descendants("ITEM");

            var listSiteUrl = siteUrl.Select(item => new SiteUrl
                {
                    Url = item.Attribute("URL").Value, 
                    Environment = item.Attribute("Environment").Value, 
                    IsDefault = item.Attribute("IsDefault").Value == "1",
                    IsActive = item.Attribute("IsActive").Value == "1"
                })
                .ToList();

            ViewBag.SiteUrl = listSiteUrl;
        }

        return View("~/Views/Trace/_NAP_Site_Url.cshtml");
    }

    public ActionResult NAPAssignPermission(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId is (int)LogAction.NAP_Group_AssignFunction or (int)LogAction.NAP_User_AssignFunction or (int)LogAction.NAP_Function_CopyPermission)
        {
            var siteId = Convert.ToInt32(elementNew.Attribute("SiteId").Value);

            ViewBag.FunctionList = functionService.GetBySiteInHierarchy(siteId, Environment, true);


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

            var site = siteService.GetById(siteId);

            ViewBag.SiteName = site.Name;
        }

        #endregion View log

        return View("~/Views/Trace/_NAP_Assign_Permission.cshtml");
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
        var documentFunctionPolicy = XDocument.Parse(strFunctionPolicy);
        var functionPolicy = documentFunctionPolicy.Descendants("ITEM");

        return functionPolicy.Select(item => new FunctionPolicy
        {
            Id = Convert.ToInt16(item.Attribute("FunctionId").Value), 
            Policy = Convert.ToInt32(item.Attribute("Policy").Value)
        }).ToList();
    }

    public ActionResult NAPAssignSite(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_User_AssignSite)
        {
            if (elementOld.Attribute("Value") != null
                && !string.IsNullOrWhiteSpace(elementOld.Attribute("Value").Value))
            {
                var strUrlOld = elementOld.Attribute("Value").Value.DecodeXMLValue();

                var documentSiteUrlOld = XDocument.Parse(strUrlOld);
                var siteUrlOld = documentSiteUrlOld.Descendants("ITEM");

                var listSiteUrlOld = siteUrlOld.Select(item => new SiteLevel
                {
                    SiteId = Convert.ToByte(item.Attribute("SiteId").Value),
                    SiteName = item.Attribute("SiteName").Value,
                    Level = Convert.ToInt32(item.Attribute("Level").Value), 
                    IsActive = item.Attribute("IsActive").Value == "1"
                }).ToList();

                ViewBag.SiteUrlOld = listSiteUrlOld;
            }

            if (elementNew.Attribute("Value") != null)
            {
                var strUrl = elementNew.Attribute("Value").Value.DecodeXMLValue();

                var documentSiteUrl = XDocument.Parse(strUrl);
                var siteUrl = documentSiteUrl.Descendants("ITEM");
                var listSiteUrl = siteUrl.Select(item => new SiteLevel
                {
                    SiteId = Convert.ToByte(item.Attribute("SiteId").Value),
                    SiteName = item.Attribute("SiteName").Value,
                    Level = Convert.ToInt32(item.Attribute("Level").Value), 
                    IsActive = item.Attribute("IsActive").Value == "1"
                }).ToList();

                ViewBag.SiteUrl = listSiteUrl;
            }
        }

        return View("~/Views/Trace/_NAP_Assign_Site.cshtml");
    }

    public ActionResult NAPAssignNewSite(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);

        if (log.ActionId == (int)LogAction.NAP_User_AssignNewSite)
        {
            var newSite = new SiteLevel
            {
                SiteId = Convert.ToByte(elementNew.Attribute("SiteId").Value),
                SiteName = Convert.ToString(elementNew.Attribute("SiteName").Value),
                Level = Convert.ToInt32(elementNew.Attribute("Level").Value),
                IsActive = elementNew.Attribute("IsActive").Value == "1"
            };

            ViewBag.NewLog = newSite;
        }

        return View("~/Views/Trace/_NAP_Assign_New_Site.cshtml");
    }

    public ActionResult NAPUpdateSite(LogExecSp log)
    {
        var newElement = XElement.Parse(log.Data);
        var oldElement = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (oldElement.Attribute("UserId") != null)
        {
            var oldSite = new SiteLevel
            {
                SiteId = Convert.ToByte(oldElement.Attribute("SiteId").Value),
                SiteName = Convert.ToString(oldElement.Attribute("SiteName").Value),
                Level = Convert.ToInt32(oldElement.Attribute("Level").Value),
                IsActive = oldElement.Attribute("IsActive").Value == "1",
                IsNeverExpired = oldElement.Attribute("IsNeverExpired").Value == "1"
            };

            DateTime.TryParse(oldElement.Attribute("ExpiredDate").Value, out var expiredDate);
            oldSite.ExpiredDate = expiredDate;

            ViewBag.oldSite = oldSite;
        }

        if (newElement.Attribute("UserId") != null)
        {
            var newSite = new SiteLevel
            {
                SiteId = Convert.ToByte(newElement.Attribute("SiteId").Value),
                SiteName = Convert.ToString(newElement.Attribute("SiteName").Value),
                Level = Convert.ToInt32(newElement.Attribute("Level").Value),
                IsActive = newElement.Attribute("IsActive").Value == "1",
                IsNeverExpired = newElement.Attribute("IsNeverExpired").Value == "1"
            };

            DateTime.TryParse(newElement.Attribute("ExpiredDate").Value, out var expiredDate);
            newSite.ExpiredDate = expiredDate;

            ViewBag.newSite = newSite;
        }

        return View("~/Views/Trace/_NAP_Update_Site.cshtml");
    }

    public ActionResult NAPFunctionAssignToUsers(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Function_AssignFunctionToUsers)
        {
            var strUserPolicyOld = elementOld.Attribute("Value").Value.DecodeXMLValue();

            var documentUserPolicyOld = XDocument.Parse(strUserPolicyOld);
            var userPolicyOld = documentUserPolicyOld.Descendants("ITEM");

            var listUserPolicyOld = userPolicyOld.Select(item => new UserPolicy
            {
                Username = item.Attribute("UserName").Value, 
                Policy = Convert.ToInt32(item.Attribute("Policy").Value)
            }).ToList();

            ViewBag.UserPolicyOld = listUserPolicyOld;

            var strUrl = elementNew.Attribute("Value").Value.DecodeXMLValue();

            var documentUserPolicy = XDocument.Parse(strUrl);
            var userPolicyNew = documentUserPolicy.Descendants("ITEM");

            var listUserPolicy = userPolicyNew.Select(item => new UserPolicy
            {
                Username = item.Attribute("UserName").Value,
                Policy = Convert.ToInt32(item.Attribute("Policy").Value)
            }).ToList();

            ViewBag.UserPolicyNew = listUserPolicy;
        }

        return View("~/Views/Trace/_NAP_Function_AssignToUsers.cshtml");
    }

    public ActionResult NAPFunctionAssignFunctionToGroups(LogExecSp log)
    {
        var elementNew = XElement.Parse(log.Data);
        var elementOld = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.NAP_Function_AssignFunctionToGroups)
        {
            var strGroupPolicyOld = elementOld.Attribute("Value").Value.DecodeXMLValue();

            var documentGroupPolicyOld = XDocument.Parse(strGroupPolicyOld);
            var groupPolicyOld = documentGroupPolicyOld.Descendants("ITEM");

            var listGroupPolicyOld = groupPolicyOld.Select(item => new GroupPolicy
            {
                Name = item.Attribute("GroupName").Value,
                Policy = Convert.ToInt32(item.Attribute("Policy").Value)
            }).ToList();

            ViewBag.GroupPolicyOld = listGroupPolicyOld;

            var strUrl = elementNew.Attribute("Value").Value.DecodeXMLValue();

            var documentGroupPolicy = XDocument.Parse(strUrl);
            var groupPolicyNew = documentGroupPolicy.Descendants("ITEM");

            var listGroupPolicy = groupPolicyNew.Select(item => new GroupPolicy
            {
                Name = item.Attribute("GroupName").Value, 
                Policy = Convert.ToInt32(item.Attribute("Policy").Value)
            }).ToList();

            ViewBag.GroupPolicyNew = listGroupPolicy;
        }

        return View("~/Views/Trace/_NAP_Function_AssignToGroups.cshtml");
    }

    public ActionResult TaskNAPRevokeUserPermission(LogExecSp log)
    {
        var taskNAPRevokeUserPermissionVM = new TaskNAPRevokeUserPermissionVM();

        var element = XElement.Parse(log.OldData ?? "<ROOT/>");

        if (log.ActionId == (int)LogAction.Task_NAP_RevokeUserPermission)
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

        return View("~/Views/Trace/_Task_NAP_RevokeUserPermission.cshtml", taskNAPRevokeUserPermissionVM);
    }
}
