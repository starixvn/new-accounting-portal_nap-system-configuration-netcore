namespace NapAdminCore.NapApiCaching;

public class DataCachingJobCommands
{
    private readonly object lockObject = new();

    private List<JobCommand> commands = [];

    public void AddUserFunctionPolicyCommand(int siteId)
    {
        var command = new JobCommand(JobCommand.UserFunctionPolicy, siteId);
        AddCommand(command);
    }

    public void AddFunctionCommand()
    {
        var command = new JobCommand(JobCommand.Function, 0);
        AddCommand(command);
    }

    public void AddGroupCommand()
    {
        var command = new JobCommand(JobCommand.Group, 0);
        AddCommand(command);
    }

    public void AddUserSiteLevelCommand()
    {
        var command = new JobCommand(JobCommand.UserSiteLevel, 0);
        AddCommand(command);
    }

    public void AddMenuCommand()
    {
        var command = new JobCommand(JobCommand.Menu, 0);
        AddCommand(command);
    }

    public void AddSiteCommand()
    {
        var command = new JobCommand(JobCommand.Site, 0);
        AddCommand(command);
    }

    public void AddUserOtpCommand()
    {
        var command = new JobCommand(JobCommand.UserOtp, 0);
        AddCommand(command);
    }

    public void AddUserCommand()
    {
        var command = new JobCommand(JobCommand.User, 0);
        AddCommand(command);
    }

    public void AddCommand(JobCommand command)
    {
        lock (lockObject)
        {
            commands.Add(command);
        }
    }

    public JobCommand PickCommand()
    {
        lock (lockObject)
        {
            return commands.OrderBy(c => c.Priority).MaxBy(c => c.Version);
        }
    }

    public void ResolveCommand(JobCommand command)
    {
        if (command == null)
        {
            return;
        }

        lock (lockObject)
        {
            commands = commands.Where(cmd => cmd.Command != command.Command || cmd.Version <= command.Version)
                .ToList();
        }
    }

    public static DataCachingJobCommands Instance { get; } = new();
}