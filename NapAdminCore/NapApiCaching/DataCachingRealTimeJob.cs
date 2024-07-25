namespace NapAdminCore.NapApiCaching;

using Fanex.Logging;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

public class DataCachingRealTimeJob(DataCachingService dataCachingService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessAllCommands(stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }

    private async Task ProcessAllCommands(CancellationToken stoppingToken)
    {
        var hasJob = false;
        var jobCommands = DataCachingJobCommands.Instance;
        var command = jobCommands.PickCommand();
        while (command != null && !stoppingToken.IsCancellationRequested)
        {
            hasJob = true;
            await TryResolveCommand(command);
            command = jobCommands.PickCommand();
        }

        if (hasJob)
        {
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task TryResolveCommand(JobCommand command)
    {
        var commandJson = JsonConvert.SerializeObject(command);
        var message = $"DataCachingRealTimeJob.TryResolveCommand({commandJson})";
        try
        {
            await ResolveCommand(command);
            Console.WriteLine(message);
        }
        catch (Exception e)
        {
            await LogService.ErrorAsync(message, e);
        }
    }

    private async Task ResolveCommand(JobCommand command)
    {
        var jobCommands = DataCachingJobCommands.Instance;
        switch (command.Command)
        {
            case JobCommand.Site:
                await dataCachingService.ReloadSites();
                break;
            case JobCommand.Function:
                await dataCachingService.ReloadFunctions();
                break;
            case JobCommand.Group:
                await dataCachingService.ReloadGroups();
                break;
            case JobCommand.Menu:
                await dataCachingService.ReloadSiteMenu();
                break;
            case JobCommand.UserFunctionPolicy:
                await dataCachingService.ReloadUserFunctionPolicy(command.SiteId);
                break;
            case JobCommand.User:
                await dataCachingService.ReloadUsers();
                break;
            case JobCommand.UserOtp:
                await dataCachingService.ReloadUserOtp();
                break;
            case JobCommand.UserSiteLevel:
                await dataCachingService.ReloadUserSiteLevels();
                break;
        }

        jobCommands.ResolveCommand(command);
    }
}