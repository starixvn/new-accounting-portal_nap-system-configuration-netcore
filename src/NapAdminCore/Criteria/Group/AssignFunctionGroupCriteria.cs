
namespace NapAdminCore.Criteria.Group;

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Fanex.Data;
using Fanex.Data.Repository;
using Web;

public class AssignFunctionGroupCriteria : CriteriaBase
{
    private const string AssignPermissionTpl = "<FunctionPolicy SiteId='{0}' FunctionId='{1}' Policy='{2}'></FunctionPolicy>";

    public int GroupId { get; set; }

    [SpParam(Ignored = true)]
    public string GroupName { get; set; }

    [Display(Name = "Site", ResourceType = typeof(ViewResources))]
    public int SiteId { get; set; }

    [SpParam(Ignored = true)]
    public int[] FuncRead { get; set; }

    [SpParam(Ignored = true)]
    public int[] FuncHost { get; set; }

    public string Value
    {
        get
        {
            var value = new StringBuilder("<ROOT>");

            if (FuncRead != null)
            {
                foreach (var funcId in FuncRead)
                {
                    value.AppendFormat(AssignPermissionTpl, SiteId, funcId, 1); // 1 -> read
                }
            }

            if (FuncHost != null)
            {
                foreach (var funcId in FuncHost)
                {
                    value.AppendFormat(AssignPermissionTpl, SiteId, funcId, 2); // 2 -> host
                }
            }

            return value.Append("</ROOT>").ToString();
        }
    }

    public int ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Group_AssignFunction";
    }

    public override bool IsValid()
    {
        return true;
    }
}
