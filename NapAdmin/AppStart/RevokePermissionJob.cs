namespace NapAdmin.AppStart;

using Fanex.Data.Repository;
using Fanex.Logging;
using NapAdminCore.Criteria;
using NapAdminCore.NapApiCaching;

public class RevokePermissionJob(ApiClientService apiClientService, ILogger logger) :  BackgroundService
{
    private IDynamicRepository repository = new DynamicRepository();
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var command = new RevokePermissionCommand();
        while (!stoppingToken.IsCancellationRequested)
        {
            await TryToRevokePermission(command);
            await TryReloadAllCache(stoppingToken);
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task TryToRevokePermission(RevokePermissionCommand command)
    {
        try
        {
            await repository.ExecuteAsync(command);
            await logger.InfoAsync($"Finish revoke permission at {DateTimeOffset.Now}");
        }
        catch (Exception e)
        {
            await logger.ErrorAsync($"Error: {e.Message} when revoke permission", e);
        }
    }

    private async Task TryReloadAllCache(CancellationToken stoppingToken)
    {
        var done = false;
        while (!done && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cacheKey = await apiClientService.ReloadAllCache();
                await logger.InfoAsync($"Finish reloading all cache. New cache key: {cacheKey}");
                done = true;
            }
            catch (Exception e)
            {
                await logger.ErrorAsync($"Error: {e.Message} when reload all cache", e);
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}