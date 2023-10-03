namespace NapAdminCore.Criteria.Site;

using System.ComponentModel.DataAnnotations;
using Fanex.Data.Repository;
using Models;
using Web;

public class UpdateSiteCriteria : CriteriaBase
{
    public UpdateSiteCriteria()
    {
        Environment = AdminConstant.Uat;
    }

    public int SiteId { get; set; }

    [Required]
    [Display(Name = "Name", ResourceType = typeof(ViewResources))]
    [StringLength(50, MinimumLength = 2,
        ErrorMessageResourceName = "InputLengthValidationMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    [RegularExpression(@"^[\w\s]{1,}$",
        ErrorMessageResourceName = "InvalidNameValidationMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    public string SiteName { get; set; }

    [Display(Name = "Description", ResourceType = typeof(ViewResources))]
    [StringLength(200, MinimumLength = 0,
        ErrorMessageResourceName = "InputLengthValidationMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    public string Description { get; set; }

    [Display(Name = "Status", ResourceType = typeof(ViewResources))]
    public bool IsActive { get; set; }

    [Required]
    [Display(Name = "Environment", ResourceType = typeof(ViewResources))]
    public string Environment { get; set; }

    [Display(Name = "Site_Key", ResourceType = typeof(ViewResources))]
    [StringLength(50, MinimumLength = 2,
        ErrorMessageResourceName = "InputLengthValidationMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    public string SiteKey { get; set; }

    [Display(Name = "Access_Secret_Key", ResourceType = typeof(ViewResources))]
    [StringLength(200, MinimumLength = 2,
        ErrorMessageResourceName = "InputLengthValidationMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    public string SecretAccessKey { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Site_Update";
    }

    public override bool IsValid()
    {
        return true;
    }
}
