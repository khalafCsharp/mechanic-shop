using Domain.Common.Results;
using Domain.Features.Customers;
using Domain.Features.Customers.Vehicles;

namespace MechanicShop.Test.Common.Customers
{
    public static class CustomerFactory
    {
        public static Result<Customer> CreateCustomer(Guid? Id = null, string? Name = null, string? PhoneNumber = null, string? Email = null, List<Vehicle>? Vehicles = null)
        {
            return Customer.Create(
                Id ?? Guid.NewGuid(),
                Name ?? "khalaf",
                PhoneNumber ?? "5544332211",
                Email ?? "khalafmajid93@gmail.com",
                Vehicles ?? [VehicleFactory.CreateVehicle().Value]
            );
        }
    }
}