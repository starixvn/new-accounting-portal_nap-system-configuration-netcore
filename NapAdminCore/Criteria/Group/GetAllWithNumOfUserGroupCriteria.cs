namespace NapAdminCore.Criteria.Group;

using System.ComponentModel.DataAnnotations;
using Fanex.Data.Repository;
using Web;

public class GetAllWithNumOfUserGroupCriteria : CriteriaBase
{
    public bool? IsActive { get; set; }

    [Display(Name = "Group_Name", ResourceType = typeof(ViewResources))]
    [StringLength(50, MinimumLength = 0,
        ErrorMessageResourceName = "InputLengthValidationMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    public string GroupName { get; set; }

    public string Environment { get; set; }

    public override string GetSettingKey() => "NAP_Group_GetAllWithNumOfUser";

    public override bool IsValid() => true;
}
