using Application.Features.Customers.Dtos;
using Domain.Features.Customers;
using Domain.Features.Customers.Vehicles;

namespace Application.Features.Customers.Mappers
{
    public static class CustomerMapper
    {
        public static CustomerDto ToDto(this Customer? customer)
        {
            if (customer is null)
            {
                throw new NullReferenceException("can not be mapped, customer is null");
            }
            return new CustomerDto()
            {
                CustomerId = customer.Id,
                Name = customer.Name ?? "",
                PhoneNumber = customer.PhoneNumber ?? "",
                Email = customer.Email ?? "",
                VehicleDtos = customer.Vehicles.ToDtos()
            };
        }
        public static List<CustomerDto> ToDtos(this IEnumerable<Customer> customers)
        {
            return [.. customers.Select(c => ToDto(c))];
        }

        public static VehicleDto ToDto(this Vehicle? vehicle)
        {
            if (vehicle is null)
            {
                throw new NullReferenceException("can not be mapped, vehicle is null");
            }
            return new VehicleDto(vehicle.Id, vehicle.Make, vehicle.Model, vehicle.Year, vehicle.LicensePlate);
        }
        public static List<VehicleDto> ToDtos(this IEnumerable<Vehicle> vehicles)
        {
            return [.. vehicles.Select(v => ToDto(v))];
        }
    }
}