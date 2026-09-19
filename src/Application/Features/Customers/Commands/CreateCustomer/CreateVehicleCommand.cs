using Application.Features.Customers.Dtos;
using Domain.Common.Results;
using MediatR;

namespace Application.Features.Customers.Commands.CreateCustomer
{
    public record CreateVehicleCommand(string Make, string Model, int Year, string LicensePlate) : IRequest<Result<VehicleDto>>;
}