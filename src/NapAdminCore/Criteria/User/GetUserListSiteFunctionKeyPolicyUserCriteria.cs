
namespace NapAdminCore.Criteria.User;

using System.Collections.Generic;
using System.Text;
using Fanex.Data;
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;

public class GetUserListSiteFunctionKeyPolicyUserCriteria : CriteriaBase
{
    private readonly string _siteTml = "<Site SiteId='{0}'></Site>";

    public int UserId { get; set; }

    public string Site
    {
        get
        {
            if (ListSite != null)
            {
                var value = new StringBuilder("<ROOT>");

                foreach (var siteId in ListSite)
                {
                    value.AppendFormat(_siteTml, siteId.Value); // 1 -> read
                }

                return value.Append("</ROOT>").ToString();
            }

            return string.Empty;
        }
    }

    [SpParam(Ignored = true)]
    public List<SelectListItem> ListSite { get; set; }

    public string FunctionKey { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_GetUserListSiteFunctionKeyPolicy";
    }

    public override bool IsValid()
    {
        return true;
    }
}
