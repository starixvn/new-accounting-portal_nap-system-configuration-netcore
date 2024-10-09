using Fanex.Data.Repository;

namespace NapAdminCore.Criteria;


public class RevokePermissionCommand: NonQueryCommand
{
    public override string GetSettingKey() => "RevokeUserPermission";

    public override bool IsValid()=> true;
}