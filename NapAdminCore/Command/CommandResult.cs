
namespace NapAdminCore.Command;

public interface ICommandResult
{
    dynamic Code { get; }

    dynamic Message { get; }

    bool IsSuccessful { get; }
}

public class CommandResultBase : ICommandResult
{
    public CommandResultBase()
        : this(string.Empty, string.Empty)
    {
    }

    public CommandResultBase(dynamic code, dynamic message)
    {
        Code = code;
        Message = message;
    }

    public dynamic Code { get; set; }

    public dynamic Message { get; set; }

    public virtual bool IsSuccessful => true;
}

public class CommandResult : CommandResultBase
{
    public override bool IsSuccessful => Code == 0;
}
