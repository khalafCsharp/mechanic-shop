using Domain.Common.Results;
using MediatR;

namespace Application.Features.Customers.Commands.UpdateCustomer
{
    public record UpdateVehicleCommand(Guid? VehicleToBeUpdatedId, string Make, string Model, int Year, string LicensePlate) : IRequest<Result<Updated>>;
}