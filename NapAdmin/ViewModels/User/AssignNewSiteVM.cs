namespace NapAdmin.ViewModels.User;

using System.Collections.Generic;
using NapAdminCore.Domain;
using NapAdminCore.Models.ValueObject;

public class AssignNewSiteVM
{
    public ReturnData ReturnData { get; set; } = new();

    public List<ISite> AvailableSitesForUser { get; set; }

    public List<SiteLevel> CurrentUserSiteLevelList { get; set; }

    public List<ISite> HostRightSites { get; set; }

    public string UserName { get; set; }

    public int UserId { get; set; }

    public int Level { get; set; }
}
