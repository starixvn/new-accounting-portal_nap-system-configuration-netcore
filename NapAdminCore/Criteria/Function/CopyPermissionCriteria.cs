namespace NapAdminCore.Criteria.Function;

using System;
using System.ComponentModel.DataAnnotations;
using Fanex.Data;
using Models;
using Web;

public class CopyPermissionCriteria : UpdateCommandBase
{
    public int FromFunctionId { get; set; }

    public int ToFunctionId { get; set; }

    public int SiteId { get; set; }

    [SpParam(Ignored = true)]
    public string ToFunctionName { get; set; }

    public int ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    [Required]
    [Display(Name = "Environment", ResourceType = typeof(ViewResources))]
    public string Environment { get; set; } = AdminConstant.Uat;

    [SpParam(Ignored = true)]
    public int ReturnSiteId { get; set; }

    public override string GetSettingKey() => "NAP_Function_CopyPermission";

    public override bool IsValid() => true;
}
