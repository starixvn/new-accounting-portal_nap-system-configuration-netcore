namespace NapAdminCore.Criteria.Function;

using System;
using System.Collections.Generic;
using System.Text;
using Fanex.Data;

public class AssignFunctionToGroupsCriteria : UpdateCommandBase
{
    private const string AssignPermissionTpl = "<FunctionPolicy GroupId='{0}' Policy='{1}'></FunctionPolicy>";

    public int FunctionId { get; set; }

    public int SiteId { get; set; }

    public string Value
    {
        get
        {
            var value = new StringBuilder("<ROOT>");

            if (GroupRead != null)
            {
                foreach (var groupId in GroupRead)
                {
                    value.AppendFormat(AssignPermissionTpl, groupId, 1); // 1 -> read
                }
            }

            if (GroupHost != null)
            {
                foreach (var userId in GroupHost)
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
    public List<string> GroupRead { get; set; }

    [SpParam(Ignored = true)]
    public List<string> GroupHost { get; set; }

    public override string GetSettingKey() => "NAP_Function_AssignFunctionToGroups";

    public override bool IsValid() => true;
}
