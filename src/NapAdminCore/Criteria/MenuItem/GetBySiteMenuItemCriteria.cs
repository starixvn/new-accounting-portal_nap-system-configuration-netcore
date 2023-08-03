
namespace NapAdminCore.Criteria.MenuItem;

using System.ComponentModel.DataAnnotations;
using Fanex.Data.Repository;
using Web;

public class GetBySiteMenuItemCriteria : CriteriaBase
{
    [Required]
    [Display(Name = "Site", ResourceType = typeof(ViewResources))]
    public int SiteId { get; set; }

    [Display(Name = "Environment", ResourceType = typeof(ViewResources))]
    public string Environment { get; set; }

    public bool? IsActive { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_MenuItem_GetBySite";
    }

    public override bool IsValid()
    {
        return true;
    }
}
