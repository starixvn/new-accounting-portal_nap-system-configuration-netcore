namespace NapAdminCore.Criteria.User;

using Fanex.Data.Repository;

public class InsertCust6RollbackGlobCommand : NonQueryCommand
{
    public int CustID { get; set; }

    public int SubAccID => 0;

    public override string GetSettingKey() => "Acc_CustomerIns_Rollback";

    public override bool IsValid() => true;
}
