namespace Infrastructure.BackgroundJobs
{
    using Application.Common.Interfaces;
    using Domain.Features.WorkOrders;
    using Infrastructure.Settings;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// creates a background job that checks for unattended workorder appointments and removes them.
    /// </summary>
    public class StaleScheduledWorkOrdersCleaningService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<StaleScheduledWorkOrdersCleaningService> logger;
        private readonly AppSettings appSettings;
        private readonly TimeProvider dateTime;

        /// <summary>
 /// Initializes a new instance of the <see cref="StaleScheduledWorkOrdersCleaningService"/> class.
        /// </summary>
        /// <param name="scopeFactory"> the scope factory to create services within scopes.</param>
        /// <param name="logger"> for logging.</param>
        /// <param name="options">Retrieves configured AppSettings instances.</param>
        /// <param name="dateTime">provides time.</param>
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

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(this.appSettings.StaleScheduledWorkOrdersCleanupFrequencyMinutes));

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                this.logger.LogInformation("Checking staled scheduled work orders at {time}", this.dateTime.GetUtcNow());

                try
                {
                    using var scope = this.scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

                    var staleBookingThresholdUtc = this.dateTime.GetUtcNow().AddMinutes(-this.appSettings.BookingCancellationThresholdMinutes);

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
                                this.logger.LogWarning("Failed to cancel WorkOrder {id}: {Errors}", swo.Id, workOrderCancellationResult.Errors);
                            }
                        }

                        await context.SaveChangesAsync(cancellationToken);

                        this.logger.LogInformation("Cancelled {count} staled work orders: {ids}", staledWorkOrders.Count, staledWorkOrders.Select(w => w.Id));
                    }
                    else
                    {
                        this.logger.LogInformation("No staled work orders found.");
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error cleaning up staled work orders.");
                }
            }
        }
    }
}