namespace NapAdminCore.Criteria.Site;

using System.ComponentModel.DataAnnotations;
using Web;

public class UpdateSiteUrlCriteria : UpdateCommandBase
{
    public int SiteUrlId { get; set; }

    [Required]
    public int SiteId { get; set; }

    [Required]
    [Display(Name = "URL", ResourceType = typeof(ViewResources))]
    [StringLength(100, MinimumLength = 0,
        ErrorMessageResourceName = "InputLengthValidationMsg", ErrorMessageResourceType = typeof(ViewResources))]
    public string Url { get; set; }

    [Required]
    [Display(Name = "Environment", ResourceType = typeof(ViewResources))]
    [StringLength(10, MinimumLength = 0,
        ErrorMessageResourceName = "InputLengthValidationMsg", ErrorMessageResourceType = typeof(ViewResources))]
    public string Environment { get; set; }

    [Display(Name = "Status", ResourceType = typeof(ViewResources))]
    public bool IsActive { get; set; }

    [Display(Name = "Default", ResourceType = typeof(ViewResources))]
    public bool IsDefault { get; set; }

    public override string GetSettingKey() => "NAP_Site_UpdateSiteURL";

    public override bool IsValid() => true;
}
