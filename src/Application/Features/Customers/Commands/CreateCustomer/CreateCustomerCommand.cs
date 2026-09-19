using Application.Features.Customers.Dtos;
using Domain.Common.Results;
using MediatR;

namespace Application.Features.Customers.Commands.CreateCustomer
{
    public record CreateCustomerCommand(string Name, string Email, string PhoneNumber, List<CreateVehicleCommand> CreateVehicleCommands) : IRequest<Result<CustomerDto>>; 
}