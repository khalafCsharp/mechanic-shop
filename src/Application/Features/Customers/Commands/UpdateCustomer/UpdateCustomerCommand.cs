using Domain.Common.Results;
using MediatR;

namespace Application.Features.Customers.Commands.UpdateCustomer
{
    public record UpdateCustomerCommand(Guid CustomerToBeUpdatedId, string Name, string Email, string PhoneNumber, List<UpdateVehicleCommand> UpdateVehicleCommands) : IRequest<Result<Updated>>;
}