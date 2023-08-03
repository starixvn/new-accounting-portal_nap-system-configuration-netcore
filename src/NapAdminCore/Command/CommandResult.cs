
namespace NapAdminCore.Command;

public interface ICommandResult
{
    dynamic Code { get; }

    dynamic Message { get; }

    bool IsSuccessful { get; }
}

public class CommandResultBase : ICommandResult
{
    private dynamic _code;
    private dynamic _message;

    public CommandResultBase()
        : this(string.Empty, string.Empty)
    {
    }

    public CommandResultBase(dynamic code, dynamic message)
    {
        _code = code;
        _message = message;
    }

    public dynamic Code
    {
        get => _code;
        set => _code = value;
    }

    public dynamic Message
    {
        get => _message;
        set => _message = value;
    }

    public virtual bool IsSuccessful => true;
}

public class CommandResult : CommandResultBase
{
    public override bool IsSuccessful => Code == 0;
}
