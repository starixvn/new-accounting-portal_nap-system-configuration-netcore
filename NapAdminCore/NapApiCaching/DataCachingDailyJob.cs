namespace NapAdminCore.NapApiCaching;

using Microsoft.Extensions.Hosting;

public class DataCachingDailyJob(DataCachingService dataCachingService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await dataCachingService.ReloadAll();
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}