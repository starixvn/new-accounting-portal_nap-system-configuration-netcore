namespace NapAdminCore.Criteria.User;

using System.ComponentModel.DataAnnotations;
using Fanex.Data;
using Fanex.Data.Repository;
using Web;

public class UpdateCustCriteria : CriteriaBase
{
    public int CustId { get; set; }

    [Display(Name = "First_Name", ResourceType = typeof(ViewResources))]
    [StringLength(50, MinimumLength = 0,
        ErrorMessageResourceName = "InputLengthValidationMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    public string FirstName { get; set; }

    [Display(Name = "Last_Name", ResourceType = typeof(ViewResources))]
    [StringLength(50, MinimumLength = 0,
        ErrorMessageResourceName = "InputLengthValidationMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    public string LastName { get; set; }

    [Display(Name = "Email", ResourceType = typeof(ViewResources))]
    [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
        ErrorMessageResourceName = "EmailFormatValidationMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    public string Email { get; set; }

    [Display(Name = "Group", ResourceType = typeof(ViewResources))]
    public int GroupId { get; set; }

    [SpParam(Ignored = true)]
    public string Username { get; set; }

    [SpParam(Ignored = true)]
    public int SiteId { get; set; }

    [SpParam(Ignored = true)]
    public int PageIndex { get; set; }

    [SpParam(Ignored = true)]
    public int ReturnGroupId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Cust_Update";
    }

    public override bool IsValid()
    {
        return true;
    }
}
