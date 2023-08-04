namespace NapAdmin.ViewModels.User;

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NapAdminCore.Criteria.User;
using NapAdminCore.Models;
using NapAdminCore.Models.ValueObject;

public class AssignPermissionVM
{
    public ReturnData ReturnData { get; set; } = new();

    public AssignFunctionUserCriteria Criteria { get; set; }

    public List<FunctionPolicy> GroupFuncPolicyList { get; set; }

    public List<SelectListItem> Sites { get; set; }

    public List<Function> FunctionList { get; set; }

    public List<FunctionPolicy> UserFuncPolicyList { get; set; }

    public List<FunctionPolicy> CurrentUserFuncPolicyList { get; set; }

    public int AdminLevel { get; set; }

    public int UserLevel { get; set; }
}
