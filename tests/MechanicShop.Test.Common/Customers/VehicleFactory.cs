using Domain.Common.Results;
using Domain.Features.Customers.Vehicles;

namespace MechanicShop.Test.Common.Customers
{
    public static class VehicleFactory
    {
        public static Result<Vehicle> CreateVehicle(Guid? Id = null, string? Make = null, string? Model = null, int? Year = null, string? LicensePlate = null)
        {
            return Vehicle.Create(
                Id ?? Guid.NewGuid(),
                Make ?? "Toyota",
                Model ?? "Camry",
                Year ?? 2000,
                LicensePlate ?? "ABC 123"
            );
        }
    }
}