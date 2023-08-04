namespace NapAdminCore.Criteria.Function;

using System;
using System.ComponentModel.DataAnnotations;
using Fanex.Data.Repository;
using NapAdminCore.Models;
using NapAdminCore.Web;

public class InsertFunctionCriteria : CriteriaBase
{
    public InsertFunctionCriteria()
    {
        this.Environment = AdminConstant.Uat;
    }

    public int FunctionId { get; set; }

    public int SiteId { get; set; }

    [Required]
    [Display(Name = "Function_Name", ResourceType = typeof(ViewResources))]
    [StringLength(50, MinimumLength = 2,
        ErrorMessageResourceName = "InputLengthValidationMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    public string FunctionName { get; set; }

    [Display(Name = "Function_Key", ResourceType = typeof(ViewResources))]
    [StringLength(50, MinimumLength = 2,
        ErrorMessageResourceName = "InputLengthValidationMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    public string FunctionKey { get; set; }

    [Display(Name = "Description", ResourceType = typeof(ViewResources))]
    [StringLength(200, MinimumLength = 0,
        ErrorMessageResourceName = "InputLengthValidationMsg",
        ErrorMessageResourceType = typeof(ViewResources))]
    public string Description { get; set; }

    public int? ParentId { get; set; }

    public string ParentName { get; set; }

    [Display(Name = "CRUD", ResourceType = typeof(ViewResources))]
    public byte Crud
    {
        get
        {
            return this.Readonly ? (byte)AdminConstant.Crud.Read : (byte)AdminConstant.Crud.Host;
        }
    }

    [Display(Name = "Read_only", ResourceType = typeof(ViewResources))]
    public bool Readonly { get; set; }

    [Required]
    [Display(Name = "Display_Order", ResourceType = typeof(ViewResources))]
    public int DisplayOrder { get; set; }

    [Required]
    [Display(Name = "Status", ResourceType = typeof(ViewResources))]
    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    [Required]
    [Display(Name = "Environment", ResourceType = typeof(ViewResources))]
    public string Environment { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Function_Insert";
    }

    public override bool IsValid()
    {
        return true;
    }
}
