namespace NapAdminCore.Criteria.Function;

using System;
using System.Collections.Generic;
using System.Text;
using Fanex.Data;
using Fanex.Data.Repository;

public class AssignFunctionToUsersCriteria : CriteriaBase
{
    private const string AssignPermissionTpl = "<FunctionPolicy UserId='{0}' Policy='{1}'></FunctionPolicy>";

    public int FunctionId { get; set; }

    public int SiteId { get; set; }

    public string Value
    {
        get
        {
            var value = new StringBuilder("<ROOT>");

            if (UserRead != null)
            {
                foreach (var userId in UserRead)
                {
                    value.AppendFormat(AssignPermissionTpl, userId, 1); // 1 -> read
                }
            }

            if (UserHost != null)
            {
                foreach (var userId in UserHost)
                {
                    value.AppendFormat(AssignPermissionTpl, userId, 2); // 2 -> host
                }
            }

            return value.Append("</ROOT>").ToString();
        }
    }

    public int ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    [SpParam(Ignored = true)]
    public List<string> UserRead { get; set; }

    [SpParam(Ignored = true)]
    public List<string> UserHost { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Function_AssignFunctionToUsers";
    }

    public override bool IsValid()
    {
        return true;
    }
}
