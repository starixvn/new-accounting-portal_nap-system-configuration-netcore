
namespace NapAdminCore.Criteria.Group;

using System;
using System.ComponentModel.DataAnnotations;
using Fanex.Data.Repository;
using Web;

public class InsertGroupCriteria : CriteriaBase
{
    public int GroupId { get; set; }

    [Required]
    [Display(Name = "Name", ResourceType = typeof(ViewResources))]
    [StringLength(50, MinimumLength = 2,
        ErrorMessageResourceName = "InputLengthValidationMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    [RegularExpression(@"^[\w\s]{1,}$",
        ErrorMessageResourceName = "InvalidNameValidationMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    public string GroupName { get; set; }

    [Display(Name = "Description", ResourceType = typeof(ViewResources))]
    [StringLength(200, MinimumLength = 0,
        ErrorMessageResourceName = "InputLengthValidationMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    public string Description { get; set; }

    [Required]
    [Display(Name = "Status", ResourceType = typeof(ViewResources))]
    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Group_Insert";
    }

    public override bool IsValid()
    {
        return true;
    }
}
