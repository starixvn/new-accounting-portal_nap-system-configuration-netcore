namespace NapAdminCore.NapApiCaching;

using Fanex.Logging;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

public class DataCachingRealTimeJob(ApiClientService apiClientService) : BackgroundService
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
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task TryResolveCommand(JobCommand command)
    {
        var commandJson = JsonConvert.SerializeObject(command);
        var message = $"DataCachingRealTimeJob.TryResolveCommand({commandJson})";
        try
        {
            var cacheVersion = await ResolveCommand(command);
            message += $" . Cache version:{cacheVersion}";
        }
        catch (Exception e)
        {
            await LogService.ErrorAsync(message, e);
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }

    private async Task<string> ResolveCommand(JobCommand command)
    {
        var jobCommands = DataCachingJobCommands.Instance;
        var cacheVersion = await apiClientService.ReloadCache(command);

        jobCommands.ResolveCommand(command);
        return cacheVersion;
    }


}