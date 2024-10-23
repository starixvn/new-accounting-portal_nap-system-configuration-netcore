namespace NapAdminCore.Criteria.User;

using Fanex.Data.Repository;

public class InsertCust5RollbackAdminCommand : NonQueryCommand
{
    public int CustID { get; set; }

    public override string GetSettingKey() => "Acc_CustomerInsUserInfo_Rollback";

    public override bool IsValid() => true;
}
