using Application.Common.Interfaces;
using Domain.Features.WorkOrders;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.BackgroundJobs
{
    public class StaleScheduledWorkOrdersCleaningService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<StaleScheduledWorkOrdersCleaningService> logger;
        private readonly AppSettings appSettings;
        private readonly TimeProvider dateTime;

        public StaleScheduledWorkOrdersCleaningService(
        IServiceScopeFactory scopeFactory,
        ILogger<StaleScheduledWorkOrdersCleaningService> logger,
        IOptions<AppSettings> options,
        TimeProvider dateTime)
        {
            this.scopeFactory = scopeFactory;
            this.logger = logger;
            this.appSettings = options.Value;
            this.dateTime = dateTime;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(appSettings.StaleScheduledWorkOrdersCleanupFrequencyMinutes));

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                logger.LogInformation("Checking staled scheduled work orders at {time}", dateTime.GetUtcNow());

                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

                    var staleBookingThresholdUtc = dateTime.GetUtcNow().AddMinutes(-appSettings.BookingCancellationThresholdMinutes);

                    var staledWorkOrders = await context.WorkOrders
                   .Where(w => w.State == WorkOrderState.Scheduled && w.StartAtUtc <= staleBookingThresholdUtc)
                   .ToListAsync(cancellationToken);

                    if (staledWorkOrders.Count > 0)
                    {
                        foreach (WorkOrder swo in staledWorkOrders)
                        {
                            var workOrderCancellationResult = swo.Cancel();

                            if (workOrderCancellationResult.IsError)
                            {
                                logger.LogWarning("Failed to cancel WorkOrder {id}: {Errors}", swo.Id, workOrderCancellationResult.Errors);
                            }
                        }
                        await context.SaveChangesAsync(cancellationToken);

                        logger.LogInformation("Cancelled {count} staled work orders: {ids}", staledWorkOrders.Count, staledWorkOrders.Select(w => w.Id));
                    }
                    else
                    {
                        logger.LogInformation("No staled work orders found.");
                    }

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error cleaning up staled work orders.");
                }
            }
        }
    }
}