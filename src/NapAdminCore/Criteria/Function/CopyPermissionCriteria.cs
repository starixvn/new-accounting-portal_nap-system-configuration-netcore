namespace NapAdminCore.Criteria.Function;

using System;
using System.ComponentModel.DataAnnotations;
using Fanex.Data;
using Fanex.Data.Repository;
using NapAdminCore.Models;
using NapAdminCore.Web;

public class CopyPermissionCriteria : CriteriaBase
{
    public CopyPermissionCriteria()
    {
        this.Environment = AdminConstant.Uat;
    }

    public int FromFunctionId { get; set; }

    public int ToFunctionId { get; set; }

    public int SiteId { get; set; }

    [SpParam(Ignored = true)]
    public string ToFunctionName { get; set; }

    public int ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    [Required]
    [Display(Name = "Environment", ResourceType = typeof(ViewResources))]
    public string Environment { get; set; }

    [SpParam(Ignored = true)]
    public int ReturnSiteId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Function_CopyPermission";
    }

    public override bool IsValid()
    {
        return true;
    }
}
