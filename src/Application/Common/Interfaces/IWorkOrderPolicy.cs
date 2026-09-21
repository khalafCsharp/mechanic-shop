using Domain.Common.Results;
using Domain.Features.WorkOrders.Enums;

namespace Application.Common.Interfaces
{
    public interface IWorkOrderPolicy
    {
        bool IsOutsideOperatingHours(DateTimeOffset startAt, TimeSpan duration);

        Task<bool> IsLaborOccupied(Guid laborId, Guid WorkOrderId, DateTimeOffset startAt, DateTimeOffset endAt);

        Task<bool> IsVehicleAlreadyScheduled(Guid vehicleId, DateTimeOffset startAt, DateTimeOffset endAt, Guid? excludedWorkOrderId = null);

        Task<Result<Success>> CheckSpotAvailabilityAsync(Spot spot, DateTimeOffset startAt, DateTimeOffset endAt, Guid? excludeWorkOrderId = null, CancellationToken cancellationToken = default);

        Result<Success> ValidateMinimumAppointmentDurationRequiredInMinutes(DateTimeOffset startAt, DateTimeOffset endAt);
    }
}