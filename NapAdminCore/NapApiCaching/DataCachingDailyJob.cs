namespace NapAdminCore.NapApiCaching;

using Fanex.Logging;
using Microsoft.Extensions.Hosting;

public class DataCachingDailyJob(DataCachingService dataCachingService, ILogger logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await TryReloadAllCache(stoppingToken);
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task TryReloadAllCache(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await dataCachingService.ReloadAll();
                return;
            }
            catch (Exception e)
            {
                await logger.ErrorAsync($"Error: {e.Message} when reload all cache", e);
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}