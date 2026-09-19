namespace Application.Features.Customers.Commands.UpdateCustomer
{
    public record UpdateVehicleCommand(Guid? VehicleToBeUpdatedId, string Make, string Model, int Year, string LicensePlate);
}