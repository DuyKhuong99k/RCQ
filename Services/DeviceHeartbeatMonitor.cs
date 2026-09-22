using Dao.Repos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services;

public sealed class DeviceHeartbeatMonitor(
    IServiceScopeFactory scopeFactory,
    ILogger<DeviceHeartbeatMonitor> logger) : BackgroundService
{
    private const int HeartbeatIntervalSeconds = 30;
    private const int OfflineGraceSeconds = 10;

    private const int OfflineTimeoutSeconds =
        HeartbeatIntervalSeconds + OfflineGraceSeconds;

    private static readonly TimeSpan CheckInterval =
        TimeSpan.FromSeconds(2);

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Device heartbeat monitor started. " +
            "Heartbeat={Heartbeat}s, OfflineTimeout={Timeout}s",
            HeartbeatIntervalSeconds,
            OfflineTimeoutSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();

                var database = scope.ServiceProvider
                    .GetRequiredService<Database>();

                var count = database.SetOfflineDevices(
                    OfflineTimeoutSeconds);

                if (count > 0)
                {
                    logger.LogInformation(
                        "Set {Count} device(s) to offline",
                        count);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error while checking device heartbeat");
            }

            try
            {
                await Task.Delay(
                    CheckInterval,
                    stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        logger.LogInformation(
            "Device heartbeat monitor stopped.");
    }
}