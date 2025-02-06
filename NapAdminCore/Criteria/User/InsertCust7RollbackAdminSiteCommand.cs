namespace NapAdminCore.Criteria.User;

using Fanex.Data.Repository;

public class InsertCust7RollbackAdminSiteCommand : NonQueryCommand
{
    public int CustID { get; set; }
    
    public override string GetSettingKey() => "Acc_AdminSiteUserIns_Rollback";

    public override bool IsValid() => true;
}
