namespace NapAdminCore.Criteria.User;

using System.ComponentModel.DataAnnotations;
using Fanex.Data;
using Fanex.Data.Repository;
using Web;

public class UpdatePasswordCustCommand : CriteriaBase
{
    public int CustId { get; set; }

    [SpParam(Ignored = true)]
    public string Username { get; set; }

    [Required]
    [Display(Name = "Old_Password", ResourceType = typeof(ViewResources))]
    [StringLength(50, MinimumLength = 2,
        ErrorMessageResourceName = "InputLengthValidationMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    [SpParam(Ignored = true)]
    public string OldUserPwd { get; set; }

    [Required]
    [Display(Name = "New_Password", ResourceType = typeof(ViewResources))]
    [StringLength(50, MinimumLength = 2,
        ErrorMessageResourceName = "InputLengthValidationMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    public string UserPwd { get; set; }

    [Required]
    [Display(Name = "Confirm_Password", ResourceType = typeof(ViewResources))]
    [StringLength(50, MinimumLength = 2,
        ErrorMessageResourceName = "InputLengthValidationMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    [SpParam(Ignored = true)]
    [Compare("UserPwd",
        ErrorMessageResourceName = "PropertiesMustMatchErrMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    public string ConfirmUserPwd { get; set; }

    [SpParam(Ignored = true)]
    public string Token { get; set; }

    public override string GetSettingKey() => "NAP_Cust_UpdatePassword";

    public override bool IsValid() => true;
}
