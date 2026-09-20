using Domain.Common.Results;
using Domain.Features.WorkOrders.Enums;

namespace Application.Common.Interfaces
{
    public interface IWorkOrderPolicy
    {
        bool IsOutsideOperatingHours(DateTimeOffset startAt, TimeSpan duration);

        Task<bool> IsLaborOccupied(Guid laborId, Guid excludedWorkOrderId, DateTimeOffset startAt, DateTimeOffset endAt);

        Task<bool> IsVehicleAlreadyScheduled(Guid vehicleId, DateTimeOffset startAt, DateTimeOffset endAt, Guid? excludedWorkOrderId = null);

        Task<Result<Success>> CheckSpotAvailabilityAsync(Spot spot, DateTimeOffset startAt, DateTimeOffset endAt, CancellationToken ct = default);

        Result<Success> ValidateMinimumAppointmentDurationRequiredInMinutes(DateTimeOffset startAt, DateTimeOffset endAt);
    }
}