
namespace NapAdminCore.Command;

public interface ICommandResult
{
    dynamic Code { get; }

    dynamic Message { get; }

    bool IsSuccessful { get; }
}

public class CommandResult(dynamic code, dynamic message) : ICommandResult
{
    public CommandResult()
        : this(string.Empty, string.Empty)
    {
    }

    public dynamic Code { get; set; } = code;

    public dynamic Message { get; set; } = message;

    public bool IsSuccessful => Code == 0;
}