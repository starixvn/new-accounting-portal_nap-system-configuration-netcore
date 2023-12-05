namespace NapAdminCore.Criteria.Function;

using System;

public class UpdateStatusFunctionCommand : UpdateCommandBase
{
    public int FunctionId { get; set; }

    public bool IsActive { get; set; }

    public int ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public override string GetSettingKey() => "NAP_Function_UpdateStatus";

    public override bool IsValid() => true;
}
