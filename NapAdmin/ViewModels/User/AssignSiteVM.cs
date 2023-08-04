namespace NapAdmin.ViewModels.User;

using System.Collections.Generic;
using NapAdminCore.Models;
using NapAdminCore.Models.ValueObject;

public class AssignSiteVM
{
    public ReturnData ReturnData { get; set; } = new();

    public List<Site> SiteList { get; set; }

    public List<SiteLevel> CurrentUserSiteLevelList { get; set; }

    public List<SiteLevel> SiteLevelList { get; set; }

    public List<Site> EditableSiteList { get; set; }

    public int CurrentSessionLevel { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; }

    public bool IsSelfEdit { get; set; }

    public int CurrentSiteId { get; set; }

    public bool IsPortalAdmin { get; set; }
}
