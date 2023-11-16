
namespace NapAdminCore.Web;

using Models;

public class ViewFlags
{
    public bool IsLive { get; set; }

    public bool HasEditRight { get; set; }

    public bool IsNapAdminSite { get; set; }

    public bool HasAssignSiteReadRight { get; set; }

    public bool HasAssignSiteRight { get; set; }

    public bool HasAssignFunctionRight { get; set; }

    public bool HasAssignFunctionToUserRight { get; set; }

    public bool HasAssignFunctionToGroupRight { get; set; }

    public bool HasManageUserRight { get; set; }

    public bool IsShowLevelColumn { get; set; }

    public bool IsShowActionColumn { get; set; }

    public bool IsReCheckin { get; set; }

    public void SetEditRight(int policy)
    {
        if (policy >= (int)AdminConstant.Policy.Host)
        {
            HasEditRight = true;
        }
    }
}
