using Application.Common.Interfaces;
using Domain.Common.Results;
using Domain.Common.Results.Errors;
using Domain.Features.WorkOrders.Enums;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services
{
    public class WorkOrderPolicy : IWorkOrderPolicy
    {
        private readonly IAppDbContext context;
        private readonly AppSettings appSettings;
        public WorkOrderPolicy(IAppDbContext context, IOptions<AppSettings> options)
        {
            this.context = context;
            this.appSettings = options.Value;

        }
        public async Task<Result<Success>> CheckSpotAvailabilityAsync(Spot spot, DateTimeOffset startAt, DateTimeOffset endAt, Guid? excludeWorkOrderId = null, CancellationToken cancellationToken = default)
        {
            var isOccupied = await context.WorkOrders.AnyAsync(
           a =>
           a.Spot == spot &&
           a.StartAtUtc < endAt &&
           a.EndAtUtc > startAt &&
            (!excludeWorkOrderId.HasValue || a.Id != excludeWorkOrderId.Value),
           cancellationToken);

            return isOccupied
                 ? Error.Conflict("MechanicShop_Spot_Full", "The selected time slot is unavailable for the requested services.")
                 : Result.Success;
        }

        public async Task<bool> IsLaborOccupied(Guid laborId, Guid excludedWorkOrderId, DateTimeOffset startAt, DateTimeOffset endAt)
        {
            return await context.WorkOrders.AnyAsync(a =>
             a.LaborId == laborId &&
            a.Id != excludedWorkOrderId &&
           a.StartAtUtc < endAt &&
           a.EndAtUtc > startAt);
        }
        public bool IsOutsideOperatingHours(DateTimeOffset startAt, TimeSpan duration)
        {
            var opening = startAt.Date.Add(appSettings.OpeningTime.ToTimeSpan());
            var closing = startAt.Date.Add(appSettings.ClosingTime.ToTimeSpan());
            var endAt = startAt + duration;

            return startAt < opening || endAt > closing;
        }

        public async Task<bool> IsVehicleAlreadyScheduled(
        Guid vehicleId,
        DateTimeOffset startAt,
        DateTimeOffset endAt,
        Guid? excludedWorkOrderId = null)
        {
            return await context.WorkOrders.AnyAsync(a =>
                a.VehicleId == vehicleId &&
                (excludedWorkOrderId == null || a.Id != excludedWorkOrderId) &&
                a.StartAtUtc < endAt &&
                a.EndAtUtc > startAt);
        }

        public Result<Success> ValidateMinimumAppointmentDurationRequiredInMinutes(DateTimeOffset startAt, DateTimeOffset endAt)
        {
            if ((endAt - startAt) < TimeSpan.FromMinutes(appSettings.MinimumAppointmentDurationInMinutes))
            {
                return Error.Conflict(
                    "WorkOrder_TooShort",
                    $"WorkOrder duration must be at least {appSettings.MinimumAppointmentDurationInMinutes} minutes.");
            }

            return Result.Success;
        }
    }
}