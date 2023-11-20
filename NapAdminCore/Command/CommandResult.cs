
namespace NapAdminCore.Command;

public interface ICommandResult
{
    dynamic Code { get; }

    dynamic Message { get; }

    bool IsSuccessful { get; }
}

public class CommandResultBase(dynamic code, dynamic message) : ICommandResult
{
    public CommandResultBase()
        : this(string.Empty, string.Empty)
    {
    }

    public dynamic Code { get; set; } = code;

    public dynamic Message { get; set; } = message;

    public virtual bool IsSuccessful => true;
}

public class CommandResult : CommandResultBase
{
    public override bool IsSuccessful => Code == 0;
}
